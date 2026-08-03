using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class EmailService : IEmailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IUnitOfWork unitOfWork, IAiService aiService, ILogger<EmailService> logger)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<ApiResponse<EmailResponse>> GenerateEmailAsync(EmailRequest request)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(request.CandidateId);
        if (candidate == null)
            return new ApiResponse<EmailResponse>(false, "Candidate not found", null);

        var job = request.JobId.HasValue ? await _unitOfWork.Jobs.GetByIdAsync(request.JobId.Value) : null;

        var body = await _aiService.GenerateEmailAsync(request.Type, candidate, job);

        var subject = request.Type switch
        {
            "ApplicationReceived" => $"Application Received - {candidate.FullName}",
            "InterviewInvitation" => $"Interview Invitation - {candidate.FullName}",
            "Rejection" => $"Update on Your Application - {candidate.FullName}",
            "OfferLetter" => $"Offer Letter - {candidate.FullName}",
            "FollowUp" => $"Following Up - {candidate.FullName}",
            _ => $"Communication - {candidate.FullName}"
        };

        _logger.LogInformation("Email generated: {Type} for candidate {Id}", request.Type, request.CandidateId);

        return new ApiResponse<EmailResponse>(true, "Email generated successfully",
            new EmailResponse(subject, body));
    }
}
