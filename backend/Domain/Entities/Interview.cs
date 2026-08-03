using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public InterviewType Type { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string MeetingLink { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid ApplicationId { get; set; }
    public Application Application { get; set; } = null!;

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;

    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid InterviewerId { get; set; }
    public User Interviewer { get; set; } = null!;

    public ICollection<InterviewFeedback> Feedback { get; set; } = new List<InterviewFeedback>();
}
