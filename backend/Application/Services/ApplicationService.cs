using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;
using DomainApp = Recruitment.Domain.Entities.Application;

namespace Recruitment.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IUnitOfWork unitOfWork, IAiService aiService, AutoMapper.IMapper mapper, ILogger<ApplicationService> logger)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<ApiResponse<ApplicationDto>> GetByIdAsync(Guid id)
    {
        var app = await _unitOfWork.Applications.Query()
            .Include(a => a.Candidate)
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null)
            return new ApiResponse<ApplicationDto>(false, "Application not found", null);

        return new ApiResponse<ApplicationDto>(true, "Success", MapApplication(app));
    }

    public async Task<ApiResponse<PaginatedResult<ApplicationDto>>> GetAllAsync(PagedRequest request)
    {
        IQueryable<Recruitment.Domain.Entities.Application> query = _unitOfWork.Applications.Query()
            .Include(a => a.Candidate)
            .Include(a => a.Job);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<ApplicationStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ApiResponse<PaginatedResult<ApplicationDto>>(true, "Success",
            new PaginatedResult<ApplicationDto>(items.Select(MapApplication), totalCount, request.Page, request.PageSize));
    }

    public async Task<ApiResponse<ApplicationDto>> CreateAsync(CreateApplicationRequest request)
    {
        var existing = await _unitOfWork.Applications.FindAsync(
            a => a.CandidateId == request.CandidateId && a.JobId == request.JobId);
        if (existing.Any())
            return new ApiResponse<ApplicationDto>(false, "Candidate already applied for this job", null);

        var candidate = await _unitOfWork.Candidates.Query()
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId);
        var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId);

        if (candidate == null || job == null)
            return new ApiResponse<ApplicationDto>(false, "Candidate or job not found", null);

        var matchScore = await _aiService.CalculateMatchScoreAsync(candidate, job);

        var application = new DomainApp
        {
            CandidateId = request.CandidateId,
            JobId = request.JobId,
            CoverLetter = request.CoverLetter?.Trim() ?? string.Empty,
            MatchScore = matchScore
        };

        await _unitOfWork.Applications.AddAsync(application);

        candidate.Status = CandidateStatus.Screening;
        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        application.Candidate = candidate;
        application.Job = job;

        _logger.LogInformation("Application created - Candidate {Cid} for Job {Jid}, MatchScore: {Score}",
            request.CandidateId, request.JobId, matchScore);

        return new ApiResponse<ApplicationDto>(true, "Application submitted successfully", MapApplication(application));
    }

    public async Task<ApiResponse<ApplicationDto>> UpdateStatusAsync(Guid id, string status)
    {
        var app = await _unitOfWork.Applications.Query()
            .Include(a => a.Candidate)
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null)
            return new ApiResponse<ApplicationDto>(false, "Application not found", null);

        if (!Enum.TryParse<ApplicationStatus>(status, true, out var appStatus))
            return new ApiResponse<ApplicationDto>(false, "Invalid application status", null);

        app.Status = appStatus;
        app.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Applications.UpdateAsync(app);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<ApplicationDto>(true, "Status updated", MapApplication(app));
    }

    private static ApplicationDto MapApplication(DomainApp application) =>
        new(
            application.Id,
            application.Status.ToString(),
            application.CoverLetter,
            application.Notes,
            application.MatchScore,
            application.CreatedAt,
            application.CandidateId,
            application.Candidate?.FullName ?? string.Empty,
            application.Candidate?.Email ?? string.Empty,
            application.JobId,
            application.Job?.Title ?? string.Empty
        );
}
