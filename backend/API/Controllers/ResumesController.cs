using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResumesController : ControllerBase
{
    private readonly ICandidateService _candidateService;
    private readonly IWebHostEnvironment _env;

    public ResumesController(ICandidateService candidateService, IWebHostEnvironment env)
    {
        _candidateService = candidateService;
        _env = env;
    }

    [HttpPost("upload/{candidateId}")]
    public async Task<ActionResult<ApiResponse<string>>> UploadResume(Guid candidateId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse<string>(false, "No file uploaded", null));

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new ApiResponse<string>(false, "File size exceeds maximum allowed limit (5MB)", null));

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".pdf" && ext != ".docx" && ext != ".doc")
            return BadRequest(new ApiResponse<string>(false, "Invalid file format. Only PDF and DOCX files are allowed.", null));

        var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Resumes");
        Directory.CreateDirectory(uploadsDir);

        var secureFileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, secureFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var result = await _candidateService.UpdateResumePathAsync(candidateId, file.FileName, filePath);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("download/{candidateId}")]
    public async Task<IActionResult> DownloadResume(Guid candidateId)
    {
        var candidateRes = await _candidateService.GetByIdAsync(candidateId);
        if (!candidateRes.Success || candidateRes.Data == null || string.IsNullOrEmpty(candidateRes.Data.ResumeFilePath))
            return NotFound("Resume file not found");

        var filePath = candidateRes.Data.ResumeFilePath;
        if (!System.IO.File.Exists(filePath))
            return NotFound("Physical file missing on server");

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var contentType = filePath.EndsWith(".pdf") ? "application/pdf" : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        return File(bytes, contentType, candidateRes.Data.ResumeFileName ?? "resume" + Path.GetExtension(filePath));
    }
}
