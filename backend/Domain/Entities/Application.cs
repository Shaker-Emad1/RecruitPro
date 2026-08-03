using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public string CoverLetter { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal MatchScore { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
