using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService) => _emailService = emailService;

    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<EmailResponse>>> Generate(EmailRequest request)
    {
        var result = await _emailService.GenerateEmailAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
