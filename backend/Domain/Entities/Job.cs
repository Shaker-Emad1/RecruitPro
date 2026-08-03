using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string RequiredSkills { get; set; } = string.Empty;
    public int MinimumExperience { get; set; }
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public string Location { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
