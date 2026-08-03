using Recruitment.Domain.Interfaces;
using Recruitment.Infrastructure.Data;

namespace Recruitment.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly Recruitment.Infrastructure.Data.RecruitmentDbContext _context;
    private ICandidateRepository? _candidates;
    private IRepository<Domain.Entities.User>? _users;
    private IRepository<Domain.Entities.Job>? _jobs;
    private IRepository<Domain.Entities.Application>? _applications;
    private IRepository<Domain.Entities.Interview>? _interviews;
    private IRepository<Domain.Entities.InterviewFeedback>? _interviewFeedbacks;
    private IRepository<Domain.Entities.Notification>? _notifications;
    private IRepository<Domain.Entities.Activity>? _activities;
    private IRepository<Domain.Entities.EmailTemplate>? _emailTemplates;
    private IRepository<Domain.Entities.CandidateSkill>? _candidateSkills;

    public UnitOfWork(Recruitment.Infrastructure.Data.RecruitmentDbContext context)
    {
        _context = context;
    }

    public ICandidateRepository Candidates =>
        _candidates ??= new CandidateRepository(_context);

    public IRepository<Domain.Entities.User> Users =>
        _users ??= new Repository<Domain.Entities.User>(_context);

    public IRepository<Domain.Entities.Job> Jobs =>
        _jobs ??= new Repository<Domain.Entities.Job>(_context);

    public IRepository<Domain.Entities.Application> Applications =>
        _applications ??= new Repository<Domain.Entities.Application>(_context);

    public IRepository<Domain.Entities.Interview> Interviews =>
        _interviews ??= new Repository<Domain.Entities.Interview>(_context);

    public IRepository<Domain.Entities.InterviewFeedback> InterviewFeedbacks =>
        _interviewFeedbacks ??= new Repository<Domain.Entities.InterviewFeedback>(_context);

    public IRepository<Domain.Entities.Notification> Notifications =>
        _notifications ??= new Repository<Domain.Entities.Notification>(_context);

    public IRepository<Domain.Entities.Activity> Activities =>
        _activities ??= new Repository<Domain.Entities.Activity>(_context);

    public IRepository<Domain.Entities.EmailTemplate> EmailTemplates =>
        _emailTemplates ??= new Repository<Domain.Entities.EmailTemplate>(_context);

    public IRepository<Domain.Entities.CandidateSkill> CandidateSkills =>
        _candidateSkills ??= new Repository<Domain.Entities.CandidateSkill>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
