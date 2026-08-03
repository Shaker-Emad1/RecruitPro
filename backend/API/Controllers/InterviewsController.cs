using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using System.Security.Claims;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewsController(IInterviewService interviewService) => _interviewService = interviewService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<InterviewDto>>>> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _interviewService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InterviewDto>>> GetById(Guid id)
    {
        var result = await _interviewService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InterviewDto>>> Create(CreateInterviewRequest request)
    {
        var result = await _interviewService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}/status")]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<InterviewDto>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req)
    {
        var result = await _interviewService.UpdateStatusAsync(id, req.Status);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/feedback")]
    public async Task<ActionResult<ApiResponse<InterviewFeedbackDto>>> AddFeedback(Guid id, CreateFeedbackRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _interviewService.AddFeedbackAsync(id, request, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("check-conflict")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckConflict([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] Guid interviewerId)
    {
        var result = await _interviewService.CheckConflictAsync(startTime, endTime, interviewerId);
        return Ok(result);
    }
}
