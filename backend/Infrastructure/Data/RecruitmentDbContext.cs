using Microsoft.EntityFrameworkCore;
using Recruitment.Domain.Entities;

namespace Recruitment.Infrastructure.Data;

public class RecruitmentDbContext : DbContext
{
    public RecruitmentDbContext(DbContextOptions<RecruitmentDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Recruitment.Domain.Entities.Application> Applications => Set<Recruitment.Domain.Entities.Application>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<InterviewFeedback> InterviewFeedbacks => Set<InterviewFeedback>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Candidate>(e =>
        {
            e.HasIndex(c => c.Email).IsUnique();
            e.Property(c => c.FullName).HasMaxLength(200).IsRequired();
            e.Property(c => c.Email).HasMaxLength(200).IsRequired();
            e.Property(c => c.Phone).HasMaxLength(30);
            e.Property(c => c.Location).HasMaxLength(200);
            e.HasIndex(c => c.Status);
            e.HasIndex(c => c.AiScore);
        });

        modelBuilder.Entity<CandidateSkill>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.Property(s => s.Category).HasMaxLength(100);
            e.Property(s => s.ProficiencyLevel).HasMaxLength(50);
            e.HasIndex(s => new { s.CandidateId, s.Name }).IsUnique();
        });

        modelBuilder.Entity<Job>(e =>
        {
            e.Property(j => j.Title).HasMaxLength(200).IsRequired();
            e.Property(j => j.Department).HasMaxLength(100);
            e.Property(j => j.Location).HasMaxLength(200);
            e.HasIndex(j => j.Status);
            e.HasIndex(j => j.Department);
        });

        modelBuilder.Entity<Recruitment.Domain.Entities.Application>(e =>
        {
            e.HasIndex(a => new { a.CandidateId, a.JobId }).IsUnique();
            e.HasIndex(a => a.Status);
            e.HasOne(a => a.Candidate).WithMany(c => c.Applications).HasForeignKey(a => a.CandidateId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Job).WithMany(j => j.Applications).HasForeignKey(a => a.JobId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Interview>(e =>
        {
            e.HasIndex(i => i.StartTime);
            e.HasIndex(i => i.InterviewerId);
            e.HasOne(i => i.Application).WithMany(a => a.Interviews).HasForeignKey(i => i.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Candidate).WithMany(c => c.Interviews).HasForeignKey(i => i.CandidateId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.Job).WithMany(j => j.Interviews).HasForeignKey(i => i.JobId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InterviewFeedback>(e =>
        {
            e.HasIndex(f => new { f.InterviewId, f.UserId }).IsUnique();
            e.HasOne(f => f.Interview).WithMany(i => i.Feedback).HasForeignKey(f => f.InterviewId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.Property(n => n.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(n => n.UserId);
            e.HasIndex(n => n.IsRead);
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.HasIndex(a => a.CreatedAt);
        });

        modelBuilder.Entity<EmailTemplate>(e =>
        {
            e.Property(t => t.Name).HasMaxLength(200).IsRequired();
            e.Property(t => t.Type).HasMaxLength(50);
        });
    }
}
