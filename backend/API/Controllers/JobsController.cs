using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using System.Security.Claims;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService) => _jobService = jobService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PaginatedResult<JobDto>>>> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _jobService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<JobDto>>> GetById(Guid id)
    {
        var result = await _jobService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<JobDto>>> Create(CreateJobRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = !string.IsNullOrEmpty(userIdStr) ? Guid.Parse(userIdStr) : Guid.Empty;
        var result = await _jobService.CreateAsync(request, userId);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<JobDto>>> Update(Guid id, UpdateJobRequest request)
    {
        var result = await _jobService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _jobService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
