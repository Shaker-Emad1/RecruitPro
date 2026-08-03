using Recruitment.Application.DTOs;

namespace Recruitment.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId);
}

public interface ICandidateService
{
    Task<ApiResponse<CandidateDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PaginatedResult<CandidateDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<CandidateDto>> CreateAsync(CreateCandidateRequest request);
    Task<ApiResponse<CandidateDto>> UpdateAsync(Guid id, UpdateCandidateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
    Task<ApiResponse<AiAnalysisResponse>> AnalyzeCandidateAsync(Guid candidateId, Guid? jobId);
    Task<ApiResponse<CandidateDto>> UpdateResumePathAsync(Guid candidateId, string fileName, string filePath);
}

public interface IJobService
{
    Task<ApiResponse<JobDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PaginatedResult<JobDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<JobDto>> CreateAsync(CreateJobRequest request, Guid userId);
    Task<ApiResponse<JobDto>> UpdateAsync(Guid id, UpdateJobRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

public interface IApplicationService
{
    Task<ApiResponse<ApplicationDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PaginatedResult<ApplicationDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<ApplicationDto>> CreateAsync(CreateApplicationRequest request);
    Task<ApiResponse<ApplicationDto>> UpdateStatusAsync(Guid id, string status);
}

public interface IInterviewService
{
    Task<ApiResponse<InterviewDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PaginatedResult<InterviewDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<InterviewDto>> CreateAsync(CreateInterviewRequest request);
    Task<ApiResponse<InterviewDto>> UpdateStatusAsync(Guid id, string status);
    Task<ApiResponse<InterviewFeedbackDto>> AddFeedbackAsync(Guid interviewId, CreateFeedbackRequest request, Guid userId);
    Task<ApiResponse<bool>> CheckConflictAsync(DateTime startTime, DateTime endTime, Guid interviewerId);
}

public interface IAnalyticsService
{
    Task<ApiResponse<AnalyticsDto>> GetDashboardAnalyticsAsync();
    Task<ApiResponse<ExtendedAnalyticsDto>> GetExtendedAnalyticsAsync();
    Task<ApiResponse<HiringFunnelDto>> GetHiringFunnelAsync();
}

public interface IEmailService
{
    Task<ApiResponse<EmailResponse>> GenerateEmailAsync(EmailRequest request);
}
