namespace Recruitment.Application.DTOs;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string Role);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, UserDto User);
public record UserDto(Guid Id, string FirstName, string LastName, string Email, string Role);

public record CandidateDto(
    Guid Id, string FullName, string Email, string Phone, string Location,
    int YearsOfExperience, string Education, string Certifications,
    string ResumeFileName, string ResumeFilePath, string LinkedInUrl, string GitHubUrl,
    string Status, string Classification, int AiScore, string AiSummary,
    string AiRecommendations, string AiStrengths, string AiWeaknesses,
    string AiInterviewQuestions, string AiHiringRecommendation,
    List<SkillDto> Skills, DateTime CreatedAt);

public record SkillDto(string Name, string Category, string ProficiencyLevel);

public record CreateCandidateRequest(
    string FullName, string Email, string Phone, string Location,
    int YearsOfExperience, string Education, string Certifications,
    string LinkedInUrl, string GitHubUrl, string PortfolioUrl,
    List<SkillDto> Skills);

public record UpdateCandidateRequest(
    string FullName, string Email, string Phone, string Location,
    int YearsOfExperience, string Education, string Certifications,
    string LinkedInUrl, string GitHubUrl, string PortfolioUrl,
    string Status, List<SkillDto> Skills);

public record JobDto(
    Guid Id, string Title, string Department, string Description,
    string Requirements, string RequiredSkills, int MinimumExperience,
    decimal SalaryMin, decimal SalaryMax, string Location, string Status,
    int ApplicationCount, DateTime CreatedAt);

public record CreateJobRequest(
    string Title, string Department, string Description, string Requirements,
    string RequiredSkills, int MinimumExperience, decimal SalaryMin,
    decimal SalaryMax, string Location);

public record UpdateJobRequest(
    string Title, string Department, string Description, string Requirements,
    string RequiredSkills, int MinimumExperience, decimal SalaryMin,
    decimal SalaryMax, string Location, string Status);

public record ApplicationDto(
    Guid Id, string Status, string CoverLetter, string Notes,
    decimal MatchScore, DateTime CreatedAt,
    Guid CandidateId, string CandidateName, string CandidateEmail,
    Guid JobId, string JobTitle);

public record CreateApplicationRequest(Guid CandidateId, Guid JobId, string CoverLetter);

public record InterviewDto(
    Guid Id, string Type, string Status, DateTime StartTime, DateTime EndTime,
    string MeetingLink, string Location, string Notes,
    Guid ApplicationId, Guid CandidateId, string CandidateName,
    Guid JobId, string JobTitle, Guid InterviewerId, string InterviewerName,
    List<InterviewFeedbackDto> Feedback);

public record InterviewFeedbackDto(
    Guid Id, int Rating, string Feedback, string Strengths,
    string Weaknesses, string Recommendation, DateTime CreatedAt, Guid UserId);

public record CreateInterviewRequest(
    string Type, DateTime StartTime, DateTime EndTime,
    string MeetingLink, string Location, string Notes,
    Guid ApplicationId, Guid CandidateId, Guid JobId, Guid InterviewerId);

public record CreateFeedbackRequest(
    int Rating, string Feedback, string Strengths,
    string Weaknesses, string Recommendation);

public record NotificationDto(
    Guid Id, string Title, string Message, string Type,
    bool IsRead, string ReferenceType, Guid? ReferenceId, DateTime CreatedAt);

public record ActivityDto(
    Guid Id, string Type, string Description, DateTime CreatedAt,
    Guid UserId, Guid? CandidateId, Guid? JobId);

public record AnalyticsDto(
    int TotalCandidates, int QualifiedCandidates, int RejectedCandidates,
    int InterviewsScheduled, int OpenPositions, double HiringConversionRate,
    double AverageCandidateScore, List<MonthlyTrend> MonthlyTrends);

public record MonthlyTrend(string Month, int Candidates, int Interviews, int Hires);

// ─── Extended Analytics DTOs ──────────────────────────────────────────────────
public record HiringFunnelDto(
    int Applications, int Screening, int Shortlisted,
    int Interview, int Offered, int Hired, int Rejected);

public record DepartmentBreakdownItem(string Department, int OpenJobs, int Applications, int Hired);

public record ExtendedAnalyticsDto(
    int TotalUsers, int TotalJobs, int OpenJobs, int ClosedJobs,
    int TotalApplications, int TotalInterviews,
    int OfferedCandidates, int HiredCandidates, int RejectedCandidates,
    double OfferAcceptanceRate, double JobFillRate, double CandidateConversionRate,
    double AverageInterviewScore, HiringFunnelDto Funnel,
    List<DepartmentBreakdownItem> DepartmentBreakdown,
    List<MonthlyTrend> MonthlyTrends);

public record ApiResponse<T>(bool Success, string Message, T? Data);
public record PagedRequest(int Page = 1, int PageSize = 20, string? Search = null, string? Status = null);
public record PaginatedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);

public record AiAnalysisRequest(Guid CandidateId, Guid? JobId);
public record AiAnalysisResponse(
    string Summary, int Score, string Classification, string Recommendations,
    string Strengths, string Weaknesses, string InterviewQuestions,
    string HiringRecommendation);

public record EmailRequest(string Type, Guid CandidateId, Guid? JobId);
public record EmailResponse(string Subject, string Body);
