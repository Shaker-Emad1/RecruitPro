using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class Candidate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Education { get; set; } = string.Empty;
    public string Certifications { get; set; } = string.Empty;
    public string ResumeFileName { get; set; } = string.Empty;
    public string ResumeFilePath { get; set; } = string.Empty;
    public string ResumeText { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;

    public CandidateStatus Status { get; set; } = CandidateStatus.New;
    public CandidateClassification Classification { get; set; } = CandidateClassification.WeakMatch;
    public int AiScore { get; set; }
    public string AiSummary { get; set; } = string.Empty;
    public string AiRecommendations { get; set; } = string.Empty;
    public string AiStrengths { get; set; } = string.Empty;
    public string AiWeaknesses { get; set; } = string.Empty;
    public string AiInterviewQuestions { get; set; } = string.Empty;
    public string AiHiringRecommendation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
