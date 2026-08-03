using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService) => _candidateService = candidateService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<CandidateDto>>>> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _candidateService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CandidateDto>>> GetById(Guid id)
    {
        var result = await _candidateService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CandidateDto>>> Create(CreateCandidateRequest request)
    {
        var result = await _candidateService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CandidateDto>>> Update(Guid id, UpdateCandidateRequest request)
    {
        var result = await _candidateService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _candidateService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/analyze")]
    public async Task<ActionResult<ApiResponse<AiAnalysisResponse>>> Analyze(Guid id, [FromQuery] Guid? jobId)
    {
        var result = await _candidateService.AnalyzeCandidateAsync(id, jobId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
