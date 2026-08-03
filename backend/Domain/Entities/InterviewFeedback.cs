namespace Recruitment.Domain.Entities;

public class InterviewFeedback
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Rating { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public string Strengths { get; set; } = string.Empty;
    public string Weaknesses { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
