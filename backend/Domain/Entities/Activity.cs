using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ActivityType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? CandidateId { get; set; }
    public Candidate? Candidate { get; set; }

    public Guid? JobId { get; set; }
    public Job? Job { get; set; }

    public Guid? ApplicationId { get; set; }
    public Application? Application { get; set; }
}
