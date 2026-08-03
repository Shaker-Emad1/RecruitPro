using Recruitment.Domain.Enums;

namespace Recruitment.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Recruiter;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public string FullName => $"{FirstName} {LastName}";
}
