using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(IUnitOfWork unitOfWork, IAiService aiService, AutoMapper.IMapper mapper, ILogger<CandidateService> logger)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<ApiResponse<CandidateDto>> GetByIdAsync(Guid id)
    {
        var candidate = await _unitOfWork.Candidates.Query()
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null)
            return new ApiResponse<CandidateDto>(false, "Candidate not found", null);

        return new ApiResponse<CandidateDto>(true, "Success", MapCandidate(candidate));
    }

    public async Task<ApiResponse<PaginatedResult<CandidateDto>>> GetAllAsync(PagedRequest request)
    {
        IQueryable<Candidate> query = _unitOfWork.Candidates.Query().Include(c => c.Skills);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(term) ||
                                     c.Email.ToLower().Contains(term) ||
                                     c.Skills.Any(s => s.Name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CandidateStatus>(request.Status, true, out var status))
            query = query.Where(c => c.Status == status);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ApiResponse<PaginatedResult<CandidateDto>>(true, "Success",
            new PaginatedResult<CandidateDto>(items.Select(MapCandidate), totalCount, request.Page, request.PageSize));
    }

    public async Task<ApiResponse<CandidateDto>> CreateAsync(CreateCandidateRequest request)
    {
        var existing = await _unitOfWork.Candidates.GetByEmailAsync(request.Email.ToLower());
        if (existing != null)
        {
            _logger.LogWarning("Duplicate candidate attempt for {Email}", request.Email);
            return new ApiResponse<CandidateDto>(false, "A candidate with this email already exists", MapCandidate(existing));
        }

        var candidate = new Candidate
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLower(),
            Phone = request.Phone.Trim(),
            Location = request.Location.Trim(),
            YearsOfExperience = request.YearsOfExperience,
            Education = request.Education?.Trim() ?? string.Empty,
            Certifications = request.Certifications?.Trim() ?? string.Empty,
            LinkedInUrl = request.LinkedInUrl?.Trim() ?? string.Empty,
            GitHubUrl = request.GitHubUrl?.Trim() ?? string.Empty,
            PortfolioUrl = request.PortfolioUrl?.Trim() ?? string.Empty
        };

        foreach (var skill in request.Skills ?? [])
        {
            candidate.Skills.Add(new CandidateSkill
            {
                Name = skill.Name,
                Category = skill.Category,
                ProficiencyLevel = skill.ProficiencyLevel
            });
        }

        await _unitOfWork.Candidates.AddAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Candidate {Name} ({Email}) created", candidate.FullName, candidate.Email);
        return new ApiResponse<CandidateDto>(true, "Candidate created successfully", MapCandidate(candidate));
    }

    public async Task<ApiResponse<CandidateDto>> UpdateAsync(Guid id, UpdateCandidateRequest request)
    {
        var candidate = await _unitOfWork.Candidates.Query()
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null)
            return new ApiResponse<CandidateDto>(false, "Candidate not found", null);

        candidate.FullName = request.FullName.Trim();
        candidate.Email = request.Email.Trim().ToLower();
        candidate.Phone = request.Phone.Trim();
        candidate.Location = request.Location.Trim();
        candidate.YearsOfExperience = request.YearsOfExperience;
        candidate.Education = request.Education?.Trim() ?? string.Empty;
        candidate.Certifications = request.Certifications?.Trim() ?? string.Empty;
        candidate.LinkedInUrl = request.LinkedInUrl?.Trim() ?? string.Empty;
        candidate.GitHubUrl = request.GitHubUrl?.Trim() ?? string.Empty;
        candidate.PortfolioUrl = request.PortfolioUrl?.Trim() ?? string.Empty;
        candidate.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<CandidateStatus>(request.Status, true, out var status))
            candidate.Status = status;

        candidate.Skills.Clear();
        foreach (var skill in request.Skills ?? [])
        {
            candidate.Skills.Add(new CandidateSkill
            {
                Name = skill.Name,
                Category = skill.Category,
                ProficiencyLevel = skill.ProficiencyLevel
            });
        }

        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<CandidateDto>(true, "Candidate updated successfully", MapCandidate(candidate));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(id);
        if (candidate == null)
            return new ApiResponse<bool>(false, "Candidate not found", false);

        await _unitOfWork.Candidates.DeleteAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Candidate {Id} deleted", id);
        return new ApiResponse<bool>(true, "Candidate deleted successfully", true);
    }

    public async Task<ApiResponse<AiAnalysisResponse>> AnalyzeCandidateAsync(Guid candidateId, Guid? jobId)
    {
        var candidate = await _unitOfWork.Candidates.Query()
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == candidateId);

        if (candidate == null)
            return new ApiResponse<AiAnalysisResponse>(false, "Candidate not found", null);

        Job? job = null;
        if (jobId.HasValue)
            job = await _unitOfWork.Jobs.GetByIdAsync(jobId.Value);

        _logger.LogInformation("Starting AI analysis for candidate {Id}", candidateId);

        var (summary, score, classification, recommendations, strengths, weaknesses, questions, hiringRec) =
            await _aiService.AnalyzeCandidateAsync(candidate, job);

        candidate.AiSummary = summary;
        candidate.AiScore = score;
        candidate.Classification = Enum.TryParse<CandidateClassification>(classification, true, out var parsedClassification)
            ? parsedClassification
            : CandidateClassification.ModerateMatch;
        candidate.AiRecommendations = recommendations;
        candidate.AiStrengths = strengths;
        candidate.AiWeaknesses = weaknesses;
        candidate.AiInterviewQuestions = questions;
        candidate.AiHiringRecommendation = hiringRec;
        candidate.UpdatedAt = DateTime.UtcNow;

        if (candidate.Status == CandidateStatus.New)
            candidate.Status = CandidateStatus.Screening;

        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        var result = new AiAnalysisResponse(summary, score, classification, recommendations, strengths, weaknesses, questions, hiringRec);
        return new ApiResponse<AiAnalysisResponse>(true, "Analysis complete", result);
    }

    public async Task<ApiResponse<CandidateDto>> UpdateResumePathAsync(Guid candidateId, string fileName, string filePath)
    {
        var candidate = await _unitOfWork.Candidates.Query()
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == candidateId);

        if (candidate == null)
            return new ApiResponse<CandidateDto>(false, "Candidate not found", null);

        candidate.ResumeFileName = fileName;
        candidate.ResumeFilePath = filePath;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<CandidateDto>(true, "Resume updated successfully", MapCandidate(candidate));
    }

    private static CandidateDto MapCandidate(Candidate candidate) =>
        new(
            candidate.Id,
            candidate.FullName,
            candidate.Email,
            candidate.Phone,
            candidate.Location,
            candidate.YearsOfExperience,
            candidate.Education,
            candidate.Certifications,
            candidate.ResumeFileName,
            candidate.LinkedInUrl,
            candidate.GitHubUrl,
            candidate.Status.ToString(),
            candidate.Classification.ToString(),
            candidate.AiScore,
            candidate.AiSummary,
            candidate.AiRecommendations,
            candidate.AiStrengths,
            candidate.AiWeaknesses,
            candidate.AiInterviewQuestions,
            candidate.AiHiringRecommendation,
            candidate.Skills.Select(s => new SkillDto(s.Name, s.Category, s.ProficiencyLevel)).ToList(),
            candidate.CreatedAt
        );
}
