using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobService> _logger;

    public JobService(IUnitOfWork unitOfWork, AutoMapper.IMapper mapper, ILogger<JobService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<JobDto>> GetByIdAsync(Guid id)
    {
        var job = await _unitOfWork.Jobs.Query()
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return new ApiResponse<JobDto>(false, "Job not found", null);

        return new ApiResponse<JobDto>(true, "Success", MapJob(job));
    }

    public async Task<ApiResponse<PaginatedResult<JobDto>>> GetAllAsync(PagedRequest request)
    {
        IQueryable<Job> query = _unitOfWork.Jobs.Query().Include(j => j.Applications);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(term) ||
                                     j.Department.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<JobStatus>(request.Status, true, out var status))
            query = query.Where(j => j.Status == status);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(j => j.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ApiResponse<PaginatedResult<JobDto>>(true, "Success",
            new PaginatedResult<JobDto>(items.Select(MapJob), totalCount, request.Page, request.PageSize));
    }

    public async Task<ApiResponse<JobDto>> CreateAsync(CreateJobRequest request, Guid userId)
    {
        var job = new Job
        {
            Title = request.Title.Trim(),
            Department = request.Department.Trim(),
            Description = request.Description.Trim(),
            Requirements = request.Requirements.Trim(),
            RequiredSkills = request.RequiredSkills.Trim(),
            MinimumExperience = request.MinimumExperience,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Location = request.Location.Trim(),
            CreatedById = userId,
            Status = JobStatus.Open
        };

        await _unitOfWork.Jobs.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {Title} created by {UserId}", job.Title, userId);
        return new ApiResponse<JobDto>(true, "Job created successfully", MapJob(job));
    }

    public async Task<ApiResponse<JobDto>> UpdateAsync(Guid id, UpdateJobRequest request)
    {
        var job = await _unitOfWork.Jobs.Query()
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return new ApiResponse<JobDto>(false, "Job not found", null);

        job.Title = request.Title.Trim();
        job.Department = request.Department.Trim();
        job.Description = request.Description.Trim();
        job.Requirements = request.Requirements.Trim();
        job.RequiredSkills = request.RequiredSkills.Trim();
        job.MinimumExperience = request.MinimumExperience;
        job.SalaryMin = request.SalaryMin;
        job.SalaryMax = request.SalaryMax;
        job.Location = request.Location.Trim();
        job.UpdatedAt = DateTime.UtcNow;

        if (!Enum.TryParse<JobStatus>(request.Status, true, out var status))
            return new ApiResponse<JobDto>(false, "Invalid job status", null);

        job.Status = status;

        await _unitOfWork.Jobs.UpdateAsync(job);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<JobDto>(true, "Job updated successfully", MapJob(job));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(id);
        if (job == null)
            return new ApiResponse<bool>(false, "Job not found", false);

        await _unitOfWork.Jobs.DeleteAsync(job);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {Id} deleted", id);
        return new ApiResponse<bool>(true, "Job deleted successfully", true);
    }

    private static JobDto MapJob(Job job) =>
        new(
            job.Id,
            job.Title,
            job.Department,
            job.Description,
            job.Requirements,
            job.RequiredSkills,
            job.MinimumExperience,
            job.SalaryMin,
            job.SalaryMax,
            job.Location,
            job.Status.ToString(),
            job.Applications?.Count ?? 0,
            job.CreatedAt
        );
}
