using FluentValidation;
using Recruitment.Application.DTOs;

namespace Recruitment.Application.Validators;

public class CreateCandidateRequestValidator : AbstractValidator<CreateCandidateRequest>
{
    public CreateCandidateRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Education).MaximumLength(500);
        RuleFor(x => x.Certifications).MaximumLength(1000);
        RuleFor(x => x.LinkedInUrl).MaximumLength(500);
        RuleFor(x => x.GitHubUrl).MaximumLength(500);
    }
}

public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Requirements).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.RequiredSkills).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MinimumExperience).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalaryMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalaryMax).GreaterThanOrEqualTo(x => x.SalaryMin)
            .WithMessage("Max salary must be >= min salary");
    }
}

public class CreateInterviewRequestValidator : AbstractValidator<CreateInterviewRequest>
{
    public CreateInterviewRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.CandidateId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.InterviewerId).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty();
        RuleFor(x => x).Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time");
        RuleFor(x => x.Type).NotEmpty().Must(t => t is "Technical" or "HR" or "Final");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain uppercase, lowercase, and digit");
        RuleFor(x => x.Role).NotEmpty().Must(r => r is "Admin" or "HRManager" or "Recruiter");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
