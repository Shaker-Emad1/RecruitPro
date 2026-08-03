using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InterviewService> _logger;

    public InterviewService(IUnitOfWork unitOfWork, AutoMapper.IMapper mapper, ILogger<InterviewService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<InterviewDto>> GetByIdAsync(Guid id)
    {
        var interview = await BuildInterviewQuery().FirstOrDefaultAsync(i => i.Id == id);
        if (interview == null)
            return new ApiResponse<InterviewDto>(false, "Interview not found", null);

        return new ApiResponse<InterviewDto>(true, "Success", MapInterview(interview));
    }

    public async Task<ApiResponse<PaginatedResult<InterviewDto>>> GetAllAsync(PagedRequest request)
    {
        var query = BuildInterviewQuery();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(i => i.Candidate.FullName.ToLower().Contains(term) ||
                                     i.Job.Title.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.StartTime)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ApiResponse<PaginatedResult<InterviewDto>>(true, "Success",
            new PaginatedResult<InterviewDto>(items.Select(MapInterview), totalCount, request.Page, request.PageSize));
    }

    public async Task<ApiResponse<InterviewDto>> CreateAsync(CreateInterviewRequest request)
    {
        var conflictCheck = await CheckConflictAsync(request.StartTime, request.EndTime, request.InterviewerId);
        if (conflictCheck.Data == true)
            return new ApiResponse<InterviewDto>(false, "Interviewer has a scheduling conflict", null);

        if (!Enum.TryParse<InterviewType>(request.Type, true, out var interviewType))
            return new ApiResponse<InterviewDto>(false, "Invalid interview type", null);

        var interview = new Interview
        {
            Type = interviewType,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MeetingLink = request.MeetingLink?.Trim() ?? string.Empty,
            Location = request.Location?.Trim() ?? string.Empty,
            Notes = request.Notes?.Trim() ?? string.Empty,
            ApplicationId = request.ApplicationId,
            CandidateId = request.CandidateId,
            JobId = request.JobId,
            InterviewerId = request.InterviewerId
        };

        await _unitOfWork.Interviews.AddAsync(interview);

        var application = await _unitOfWork.Applications.GetByIdAsync(request.ApplicationId);
        if (application != null)
        {
            application.Status = ApplicationStatus.Interviewed;
            await _unitOfWork.Applications.UpdateAsync(application);
        }

        await _unitOfWork.SaveChangesAsync();

        var createdInterview = await BuildInterviewQuery().FirstAsync(i => i.Id == interview.Id);
        _logger.LogInformation("Interview scheduled - Candidate {Cid}, Interviewer {Iid}, Type {Type}",
            request.CandidateId, request.InterviewerId, request.Type);

        return new ApiResponse<InterviewDto>(true, "Interview scheduled successfully", MapInterview(createdInterview));
    }

    public async Task<ApiResponse<InterviewDto>> UpdateStatusAsync(Guid id, string status)
    {
        var interview = await BuildInterviewQuery().FirstOrDefaultAsync(i => i.Id == id);
        if (interview == null)
            return new ApiResponse<InterviewDto>(false, "Interview not found", null);

        if (!Enum.TryParse<InterviewStatus>(status, true, out var interviewStatus))
            return new ApiResponse<InterviewDto>(false, "Invalid interview status", null);

        interview.Status = interviewStatus;
        interview.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Interviews.UpdateAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse<InterviewDto>(true, "Status updated", MapInterview(interview));
    }

    public async Task<ApiResponse<InterviewFeedbackDto>> AddFeedbackAsync(Guid interviewId, CreateFeedbackRequest request, Guid userId)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(interviewId);
        if (interview == null)
            return new ApiResponse<InterviewFeedbackDto>(false, "Interview not found", null);

        var existingFeedback = await _unitOfWork.InterviewFeedbacks.FindAsync(f => f.InterviewId == interviewId && f.UserId == userId);
        if (existingFeedback.Any())
            return new ApiResponse<InterviewFeedbackDto>(false, "Feedback already submitted for this interview", null);

        var feedback = new InterviewFeedback
        {
            Rating = request.Rating,
            Feedback = request.Feedback?.Trim() ?? string.Empty,
            Strengths = request.Strengths?.Trim() ?? string.Empty,
            Weaknesses = request.Weaknesses?.Trim() ?? string.Empty,
            Recommendation = request.Recommendation?.Trim() ?? string.Empty,
            InterviewId = interviewId,
            UserId = userId
        };

        await _unitOfWork.InterviewFeedbacks.AddAsync(feedback);
        interview.Status = InterviewStatus.Completed;
        await _unitOfWork.Interviews.UpdateAsync(interview);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Feedback submitted for interview {Id} by user {UserId}", interviewId, userId);

        return new ApiResponse<InterviewFeedbackDto>(true, "Feedback submitted",
            new InterviewFeedbackDto(feedback.Id, feedback.Rating, feedback.Feedback, feedback.Strengths, feedback.Weaknesses, feedback.Recommendation, feedback.CreatedAt, feedback.UserId));
    }

    public async Task<ApiResponse<bool>> CheckConflictAsync(DateTime startTime, DateTime endTime, Guid interviewerId)
    {
        var conflicts = await _unitOfWork.Interviews.FindAsync(i =>
            i.InterviewerId == interviewerId &&
            i.Status != InterviewStatus.Cancelled &&
            ((i.StartTime <= startTime && i.EndTime > startTime) ||
             (i.StartTime < endTime && i.EndTime >= endTime) ||
             (i.StartTime >= startTime && i.EndTime <= endTime)));

        return new ApiResponse<bool>(true, "Conflict check complete", conflicts.Any());
    }

    private IQueryable<Interview> BuildInterviewQuery() =>
        _unitOfWork.Interviews.Query()
            .Include(i => i.Candidate)
            .Include(i => i.Job)
            .Include(i => i.Interviewer)
            .Include(i => i.Feedback);

    private static InterviewDto MapInterview(Interview interview) =>
        new(
            interview.Id,
            interview.Type.ToString(),
            interview.Status.ToString(),
            interview.StartTime,
            interview.EndTime,
            interview.MeetingLink,
            interview.Location,
            interview.Notes,
            interview.ApplicationId,
            interview.CandidateId,
            interview.Candidate?.FullName ?? string.Empty,
            interview.JobId,
            interview.Job?.Title ?? string.Empty,
            interview.InterviewerId,
            interview.Interviewer?.FullName ?? string.Empty,
            interview.Feedback.Select(f => new InterviewFeedbackDto(f.Id, f.Rating, f.Feedback, f.Strengths, f.Weaknesses, f.Recommendation, f.CreatedAt, f.UserId)).ToList()
        );
}
