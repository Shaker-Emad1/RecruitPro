using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ICandidateService _candidateService;
    private readonly IApplicationService _applicationService;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ICandidateService candidateService, IApplicationService applicationService, ILogger<WebhookController> logger)
    {
        _candidateService = candidateService;
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpPost("candidate-submission")]
    public async Task<ActionResult<ApiResponse<CandidateDto>>> CandidateSubmission([FromBody] CreateCandidateRequest request)
    {
        _logger.LogInformation("Webhook: Candidate submission received for {Email}", request.Email);
        var result = await _candidateService.CreateAsync(request);
        if (result.Success && result.Data != null)
        {
            await _candidateService.AnalyzeCandidateAsync(result.Data.Id, null);
        }
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("application-submission")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> ApplicationSubmission([FromBody] CreateApplicationRequest request)
    {
        _logger.LogInformation("Webhook: Application submission received for candidate {Cid} job {Jid}", request.CandidateId, request.JobId);
        var result = await _applicationService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
