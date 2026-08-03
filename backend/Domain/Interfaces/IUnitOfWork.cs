using Recruitment.Domain.Entities;

namespace Recruitment.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICandidateRepository Candidates { get; }
    IRepository<User> Users { get; }
    IRepository<Job> Jobs { get; }
    IRepository<Application> Applications { get; }
    IRepository<Interview> Interviews { get; }
    IRepository<InterviewFeedback> InterviewFeedbacks { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<Activity> Activities { get; }
    IRepository<EmailTemplate> EmailTemplates { get; }
    IRepository<CandidateSkill> CandidateSkills { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
