using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService) => _analyticsService = analyticsService;

    /// <summary>Basic dashboard analytics (existing endpoint preserved)</summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<AnalyticsDto>>> GetDashboard()
    {
        var result = await _analyticsService.GetDashboardAnalyticsAsync();
        return Ok(result);
    }

    /// <summary>Extended executive analytics with all HR metrics</summary>
    [HttpGet("extended")]
    public async Task<ActionResult<ApiResponse<ExtendedAnalyticsDto>>> GetExtended()
    {
        var result = await _analyticsService.GetExtendedAnalyticsAsync();
        return Ok(result);
    }

    /// <summary>Hiring funnel stage counts</summary>
    [HttpGet("funnel")]
    public async Task<ActionResult<ApiResponse<HiringFunnelDto>>> GetFunnel()
    {
        var result = await _analyticsService.GetHiringFunnelAsync();
        return Ok(result);
    }
}
