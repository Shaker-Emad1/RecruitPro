using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<AnalyticsDto>> GetDashboardAnalyticsAsync()
    {
        var candidatesQuery = _unitOfWork.Candidates.Query();
        var interviewsQuery = _unitOfWork.Interviews.Query();
        var jobsQuery = _unitOfWork.Jobs.Query();

        var totalCandidates = candidatesQuery.Count();
        var qualifiedCandidates = candidatesQuery.Count(c =>
            c.Status == CandidateStatus.Shortlisted ||
            c.Status == CandidateStatus.Interviewed ||
            c.Status == CandidateStatus.Hired);
        var rejectedCandidates = candidatesQuery.Count(c => c.Status == CandidateStatus.Rejected);
        var interviewsScheduled = interviewsQuery.Count(i => i.Status != InterviewStatus.Cancelled);
        var openPositions = jobsQuery.Count(j => j.Status == JobStatus.Open);
        var hiredCandidates = candidatesQuery.Count(c => c.Status == CandidateStatus.Hired);
        var hiringConversionRate = totalCandidates > 0
            ? Math.Round((double)hiredCandidates / totalCandidates * 100, 2)
            : 0;
        var averageScore = totalCandidates > 0
            ? Math.Round(candidatesQuery.Average(c => (double)c.AiScore), 1)
            : 0;

        var monthlyTrends = Enumerable.Range(0, 12).Select(i =>
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            var monthStart = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            return new MonthlyTrend(
                monthStart.ToString("yyyy-MM"),
                candidatesQuery.Count(c => c.CreatedAt >= monthStart && c.CreatedAt < monthEnd),
                interviewsQuery.Count(i => i.CreatedAt >= monthStart && i.CreatedAt < monthEnd),
                candidatesQuery.Count(c => c.Status == CandidateStatus.Hired &&
                                      c.UpdatedAt.HasValue &&
                                      c.UpdatedAt.Value >= monthStart &&
                                      c.UpdatedAt.Value < monthEnd)
            );
        }).OrderBy(m => m.Month).ToList();

        var analytics = new AnalyticsDto(
            totalCandidates, qualifiedCandidates, rejectedCandidates,
            interviewsScheduled, openPositions, hiringConversionRate,
            averageScore, monthlyTrends);

        _logger.LogInformation("Analytics generated - Total: {Total}, Qualified: {Qual}, Hired: {Hired}",
            totalCandidates, qualifiedCandidates, hiredCandidates);

        return new ApiResponse<AnalyticsDto>(true, "Success", analytics);
    }

    public async Task<ApiResponse<HiringFunnelDto>> GetHiringFunnelAsync()
    {
        var appQuery = _unitOfWork.Applications.Query();

        var funnel = new HiringFunnelDto(
            Applications: appQuery.Count(),
            Screening:    appQuery.Count(a => a.Status == ApplicationStatus.Screening ||
                                              a.Status == ApplicationStatus.Reviewed ||
                                              a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Shortlisted:  appQuery.Count(a => a.Status == ApplicationStatus.Reviewed ||
                                              a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Interview:    appQuery.Count(a => a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Offered:      appQuery.Count(a => a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Hired:        appQuery.Count(a => a.Status == ApplicationStatus.Accepted),
            Rejected:     appQuery.Count(a => a.Status == ApplicationStatus.Rejected)
        );

        return new ApiResponse<HiringFunnelDto>(true, "Success", funnel);
    }

    public async Task<ApiResponse<ExtendedAnalyticsDto>> GetExtendedAnalyticsAsync()
    {
        var candidatesQuery = _unitOfWork.Candidates.Query();
        var jobsQuery        = _unitOfWork.Jobs.Query();
        var appQuery         = _unitOfWork.Applications.Query();
        var interviewsQuery  = _unitOfWork.Interviews.Query();
        var usersQuery       = _unitOfWork.Users.Query();
        var feedbackQuery    = _unitOfWork.InterviewFeedbacks.Query();

        var totalCandidates      = candidatesQuery.Count();
        var hiredCandidates      = candidatesQuery.Count(c => c.Status == CandidateStatus.Hired);
        var rejectedCandidates   = candidatesQuery.Count(c => c.Status == CandidateStatus.Rejected);
        var offeredCandidates    = appQuery.Count(a => a.Status == ApplicationStatus.Offered || a.Status == ApplicationStatus.Accepted);
        var totalJobs            = jobsQuery.Count();
        var openJobs             = jobsQuery.Count(j => j.Status == JobStatus.Open);
        var closedJobs           = jobsQuery.Count(j => j.Status == JobStatus.Closed);
        var totalApplications    = appQuery.Count();
        var totalInterviews      = interviewsQuery.Count();
        var totalUsers           = usersQuery.Count();

        var offerAcceptanceRate  = offeredCandidates > 0
            ? Math.Round((double)hiredCandidates / offeredCandidates * 100, 1) : 0;
        var jobFillRate          = totalJobs > 0
            ? Math.Round((double)closedJobs / totalJobs * 100, 1) : 0;
        var candidateConversionRate = totalCandidates > 0
            ? Math.Round((double)hiredCandidates / totalCandidates * 100, 1) : 0;

        var avgScore = feedbackQuery.Any()
            ? Math.Round(feedbackQuery.Average(f => (double)f.Rating), 1) : 0;

        // Hiring funnel
        var funnel = new HiringFunnelDto(
            Applications: totalApplications,
            Screening:    appQuery.Count(a => a.Status == ApplicationStatus.Screening ||
                                              a.Status == ApplicationStatus.Reviewed ||
                                              a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Shortlisted:  appQuery.Count(a => a.Status == ApplicationStatus.Reviewed ||
                                              a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Interview:    appQuery.Count(a => a.Status == ApplicationStatus.Interviewed ||
                                              a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Offered:      appQuery.Count(a => a.Status == ApplicationStatus.Offered ||
                                              a.Status == ApplicationStatus.Accepted),
            Hired:        appQuery.Count(a => a.Status == ApplicationStatus.Accepted),
            Rejected:     appQuery.Count(a => a.Status == ApplicationStatus.Rejected)
        );

        // Department breakdown
        var departments = jobsQuery.Select(j => j.Department).Distinct().ToList();
        var deptBreakdown = departments.Select(dept => new DepartmentBreakdownItem(
            Department:   dept,
            OpenJobs:     jobsQuery.Count(j => j.Department == dept && j.Status == JobStatus.Open),
            Applications: appQuery.Count(a => _unitOfWork.Jobs.Query()
                                .Where(j => j.Department == dept)
                                .Select(j => j.Id)
                                .Contains(a.JobId)),
            Hired:        candidatesQuery.Count(c => c.Status == CandidateStatus.Hired)
        )).ToList();

        // Monthly trends (last 12 months)
        var monthlyTrends = Enumerable.Range(0, 12).Select(i =>
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            var s = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var e = s.AddMonths(1);
            return new MonthlyTrend(
                s.ToString("yyyy-MM"),
                candidatesQuery.Count(c => c.CreatedAt >= s && c.CreatedAt < e),
                interviewsQuery.Count(i2 => i2.CreatedAt >= s && i2.CreatedAt < e),
                appQuery.Count(a => a.Status == ApplicationStatus.Accepted && a.UpdatedAt >= s && a.UpdatedAt < e)
            );
        }).OrderBy(m => m.Month).ToList();

        var result = new ExtendedAnalyticsDto(
            TotalUsers: totalUsers, TotalJobs: totalJobs,
            OpenJobs: openJobs, ClosedJobs: closedJobs,
            TotalApplications: totalApplications, TotalInterviews: totalInterviews,
            OfferedCandidates: offeredCandidates, HiredCandidates: hiredCandidates,
            RejectedCandidates: rejectedCandidates,
            OfferAcceptanceRate: offerAcceptanceRate,
            JobFillRate: jobFillRate,
            CandidateConversionRate: candidateConversionRate,
            AverageInterviewScore: avgScore,
            Funnel: funnel,
            DepartmentBreakdown: deptBreakdown,
            MonthlyTrends: monthlyTrends
        );

        return new ApiResponse<ExtendedAnalyticsDto>(true, "Success", result);
    }
}
