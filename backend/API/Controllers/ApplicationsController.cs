using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

public record UpdateStatusRequest(string Status);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService) => _applicationService = applicationService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ApplicationDto>>>> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _applicationService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> GetById(Guid id)
    {
        var result = await _applicationService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> Create(CreateApplicationRequest request)
    {
        var result = await _applicationService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}/status")]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest req)
    {
        var result = await _applicationService.UpdateStatusAsync(id, req.Status);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
