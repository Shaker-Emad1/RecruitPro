using Microsoft.EntityFrameworkCore;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Enums;

namespace Recruitment.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(DbContext context)
    {
        if (await context.Set<User>().AnyAsync()) return;

        var random = new Random(42);
        var now = DateTime.UtcNow;

        var users = new List<User>
        {
            new() { FirstName = "Admin", LastName = "User", Email = "admin@recruit.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = UserRole.Admin, CreatedAt = now.AddMonths(-12) },
            new() { FirstName = "Sarah", LastName = "Johnson", Email = "sarah@recruit.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("HrManager123!"), Role = UserRole.HRManager, CreatedAt = now.AddMonths(-12) },
            new() { FirstName = "Michael", LastName = "Chen", Email = "michael@recruit.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"), Role = UserRole.Recruiter, CreatedAt = now.AddMonths(-11) },
            new() { FirstName = "Emily", LastName = "Rodriguez", Email = "emily@recruit.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"), Role = UserRole.Recruiter, CreatedAt = now.AddMonths(-11) },
            new() { FirstName = "David", LastName = "Park", Email = "david@recruit.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Recruiter123!"), Role = UserRole.Recruiter, CreatedAt = now.AddMonths(-10) }
        };
        context.Set<User>().AddRange(users);
        await context.SaveChangesAsync();

        var firstNames = new[] { "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Lisa", "Daniel", "Nancy", "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra", "Donald", "Ashley", "Steven", "Dorothy", "Andrew", "Kimberly", "Paul", "Emily", "Joshua", "Donna", "Kenneth", "Michelle", "Kevin", "Carol", "Brian", "Amanda", "George", "Melissa", "Timothy", "Deborah" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts" };
        var skillsPool = new[] { "C#", "Java", "Python", "JavaScript", "TypeScript", "React", "Angular", "Vue.js", "Node.js", "ASP.NET Core", "SQL Server", "PostgreSQL", "MongoDB", "Docker", "Kubernetes", "AWS", "Azure", "GCP", "Git", "REST API", "GraphQL", "Redis", "RabbitMQ", "CI/CD", "Agile", "Scrum", "Machine Learning", "Deep Learning", "NLP", "Computer Vision", "TensorFlow", "PyTorch", "React Native", "Flutter", "Swift", "Kotlin", "Go", "Rust", "PHP", "Ruby", "C++", "HTML/CSS", "Figma", "UI/UX", "Product Management", "Data Analysis", "DevOps", "Selenium", "Jest", "Cypress" };
        var categories = new[] { "Technology", "Healthcare", "Finance", "Education", "E-commerce", "Consulting", "Manufacturing", "Media", "Telecom", "Energy" };
        var jobTitles = new[] { "Senior .NET Developer", "Full Stack Engineer", "Data Scientist", "DevOps Engineer", "Product Manager", "UX Designer", "Frontend Developer", "Backend Developer", "Machine Learning Engineer", "Cloud Architect", "Mobile Developer", "QA Engineer", "Software Engineer", "AI Engineer", "Security Engineer", "Technical Lead", "Engineering Manager", "Solutions Architect", "Database Administrator", "Systems Engineer" };
        var departments = new[] { "Engineering", "Product", "Design", "Data Science", "DevOps", "Security", "Mobile", "QA", "Architecture", "Research" };
        var statuses = new[] { CandidateStatus.New, CandidateStatus.Screening, CandidateStatus.Shortlisted, CandidateStatus.Interviewed, CandidateStatus.Offered, CandidateStatus.Hired, CandidateStatus.Rejected, CandidateStatus.Withdrawn };
        var classifications = new[] { CandidateClassification.WeakMatch, CandidateClassification.ModerateMatch, CandidateClassification.StrongMatch };
        var appStatuses = new[] { ApplicationStatus.Applied, ApplicationStatus.Screening, ApplicationStatus.Reviewed, ApplicationStatus.Interviewed, ApplicationStatus.Offered, ApplicationStatus.Accepted, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn };
        var interviewTypes = new[] { InterviewType.HR, InterviewType.Technical, InterviewType.Final };
        var interviewStatuses = new[] { InterviewStatus.Scheduled, InterviewStatus.Confirmed, InterviewStatus.Completed, InterviewStatus.Cancelled, InterviewStatus.NoShow };

        // 100 Jobs
        var jobs = new List<Job>();
        for (int i = 0; i < 100; i++)
        {
            var createdAt = now.AddMonths(-random.Next(1, 12)).AddDays(-random.Next(0, 28));
            var job = new Job
            {
                Title = jobTitles[random.Next(jobTitles.Length)],
                Department = departments[random.Next(departments.Length)],
                Description = $"We are looking for an experienced professional to join our {departments[random.Next(departments.Length)]} team. The ideal candidate will have strong technical skills and a passion for building great products.",
                Requirements = $"Bachelor's degree in Computer Science or related field. {random.Next(2, 8)}+ years of experience. Strong problem-solving skills and excellent communication abilities.",
                RequiredSkills = string.Join(", ", Enumerable.Range(0, random.Next(3, 6)).Select(_ => skillsPool[random.Next(skillsPool.Length)])),
                MinimumExperience = random.Next(1, 8),
                SalaryMin = 70000 + random.Next(0, 10) * 10000,
                SalaryMax = 120000 + random.Next(0, 10) * 10000,
                Location = new[] { "New York, NY", "San Francisco, CA", "Austin, TX", "Seattle, WA", "Chicago, IL", "Remote", "Boston, MA", "Denver, CO" }[random.Next(8)],
                Status = random.NextDouble() > 0.2 ? JobStatus.Open : (random.NextDouble() > 0.5 ? JobStatus.Closed : JobStatus.Draft),
                CreatedById = users[random.Next(users.Count)].Id,
                CreatedAt = createdAt,
                UpdatedAt = random.NextDouble() > 0.5 ? createdAt.AddDays(random.Next(1, 60)) : null
            };
            jobs.Add(job);
        }
        context.Set<Job>().AddRange(jobs);
        await context.SaveChangesAsync();

        // 1000 Candidates with skills
        var candidates = new List<Candidate>();
        for (int i = 0; i < 1000; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var createdAt = now.AddMonths(-random.Next(1, 12)).AddDays(-random.Next(0, 28));
            var skillCount = random.Next(3, 10);
            var candidateSkills = Enumerable.Range(0, skillCount).Select(_ => skillsPool[random.Next(skillsPool.Length)]).Distinct().ToList();
            var score = random.Next(20, 100);
            var yearsOfExp = random.Next(0, 15);

            var candidate = new Candidate
            {
                FullName = $"{firstName} {lastName}",
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{random.Next(1, 999)}@email.com",
                Phone = $"+1-{random.Next(200, 999)}-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                Location = new[] { "New York, NY", "San Francisco, CA", "Austin, TX", "Seattle, WA", "Chicago, IL", "Remote", "Boston, MA", "Denver, CO", "Los Angeles, CA", "Portland, OR" }[random.Next(10)],
                YearsOfExperience = yearsOfExp,
                Education = new[] { "B.S. Computer Science", "B.S. Information Technology", "M.S. Computer Science", "B.A. Mathematics", "Self-taught", "Bootcamp Graduate", "Ph.D. Computer Science", "B.S. Electrical Engineering" }[random.Next(8)],
                Certifications = random.NextDouble() > 0.6 ? string.Join(", ", Enumerable.Range(0, random.Next(1, 4)).Select(_ => new[] { "AWS Certified", "Azure Certified", "Google Cloud Certified", "PMP", "Scrum Master", "ISTQB", "CISSP", "CEH" }[random.Next(8)])) : "",
                Status = statuses[random.Next(statuses.Length)],
                Classification = score >= 70 ? CandidateClassification.StrongMatch : score >= 40 ? CandidateClassification.ModerateMatch : CandidateClassification.WeakMatch,
                AiScore = score,
                AiSummary = $"{firstName} {lastName} has {yearsOfExp} years of experience with expertise in {string.Join(", ", candidateSkills.Take(3))}.",
                LinkedInUrl = $"https://linkedin.com/in/{firstName.ToLower()}.{lastName.ToLower()}",
                GitHubUrl = random.NextDouble() > 0.3 ? $"https://github.com/{firstName.ToLower()}{lastName.ToLower()}" : "",
                CreatedAt = createdAt,
                UpdatedAt = random.NextDouble() > 0.4 ? createdAt.AddDays(random.Next(1, 90)) : null
            };
            candidate.Skills = candidateSkills.Select(s => new CandidateSkill
            {
                Name = s,
                Category = new[] { "Programming Language", "Framework", "Database", "Cloud", "Tool", "Methodology" }[random.Next(6)],
                ProficiencyLevel = new[] { "Beginner", "Intermediate", "Advanced", "Expert" }[random.Next(4)],
                CandidateId = candidate.Id
            }).ToList();

            candidates.Add(candidate);
        }
        context.Set<Candidate>().AddRange(candidates);
        await context.SaveChangesAsync();

        // 500 Applications
        var applications = new List<Recruitment.Domain.Entities.Application>();
        for (int i = 0; i < 500; i++)
        {
            var candidate = candidates[random.Next(candidates.Count)];
            var job = jobs[random.Next(jobs.Count)];
            if (applications.Any(a => a.CandidateId == candidate.Id && a.JobId == job.Id)) continue;

            var createdAt = candidate.CreatedAt.AddDays(random.Next(1, 30));
            applications.Add(new Recruitment.Domain.Entities.Application
            {
                CandidateId = candidate.Id,
                JobId = job.Id,
                CoverLetter = random.NextDouble() > 0.5 ? $"Dear Hiring Team,\n\nI am writing to express my interest in the {job.Title} position. With my background in {string.Join(", ", candidate.Skills.Take(3).Select(s => s.Name))}, I believe I would be a strong fit for your team.\n\nBest regards,\n{candidate.FullName}" : "",
                MatchScore = random.Next(20, 100),
                Status = appStatuses[random.Next(appStatuses.Length)],
                CreatedAt = createdAt,
                UpdatedAt = random.NextDouble() > 0.5 ? createdAt.AddDays(random.Next(1, 30)) : null
            });
        }
        context.Set<Recruitment.Domain.Entities.Application>().AddRange(applications);
        await context.SaveChangesAsync();

        var savedApplications = await context.Set<Recruitment.Domain.Entities.Application>().ToListAsync();

        // 300 Interviews
        var interviews = new List<Interview>();
        for (int i = 0; i < 300; i++)
        {
            var app = savedApplications[random.Next(savedApplications.Count)];
            var type = interviewTypes[random.Next(interviewTypes.Length)];
            var startTime = app.CreatedAt.AddDays(random.Next(3, 30)).AddHours(random.Next(8, 17));
            var endTime = startTime.AddHours(1);

            interviews.Add(new Interview
            {
                ApplicationId = app.Id,
                CandidateId = app.CandidateId,
                JobId = app.JobId,
                InterviewerId = users[random.Next(users.Count)].Id,
                Type = type,
                Status = startTime < now ? (random.NextDouble() > 0.2 ? InterviewStatus.Completed : InterviewStatus.Cancelled) : InterviewStatus.Scheduled,
                StartTime = startTime,
                EndTime = endTime,
                MeetingLink = startTime >= now ? $"https://meet.google.com/{Guid.NewGuid().ToString()[..8]}" : "",
                Location = startTime >= now ? "" : "Office A",
                Notes = "",
                CreatedAt = app.CreatedAt.AddDays(1)
            });
        }
        context.Set<Interview>().AddRange(interviews);
        await context.SaveChangesAsync();

        var savedInterviews = await context.Set<Interview>().ToListAsync();

        // Feedback for completed interviews
        var feedbacks = new List<InterviewFeedback>();
        foreach (var interview in savedInterviews.Where(i => i.Status == InterviewStatus.Completed))
        {
            var rate = random.Next(2, 6);
            feedbacks.Add(new InterviewFeedback
            {
                InterviewId = interview.Id,
                UserId = interview.InterviewerId,
                Rating = rate,
                Feedback = rate >= 4 ? "Excellent candidate with strong technical skills." : "Candidate has potential but needs improvement in some areas.",
                Strengths = string.Join(", ", Enumerable.Range(0, random.Next(1, 4)).Select(_ => new[] { "Communication", "Technical skills", "Problem-solving", "Teamwork", "Leadership", "Adaptability" }[random.Next(6)])),
                Weaknesses = rate < 4 ? string.Join(", ", Enumerable.Range(0, random.Next(1, 3)).Select(_ => new[] { "Experience gap", "Technical depth", "Cultural fit", "Communication" }[random.Next(4)])) : "",
                Recommendation = rate >= 4 ? "Strongly recommend hiring" : "Consider for another role",
                CreatedAt = interview.EndTime.AddHours(1)
            });
        }
        context.Set<InterviewFeedback>().AddRange(feedbacks);
        await context.SaveChangesAsync();

        // 200 Notifications
        var notifications = new List<Notification>();
        for (int i = 0; i < 200; i++)
        {
            var user = users[random.Next(users.Count)];
            var createdAt = now.AddMonths(-random.Next(0, 12)).AddDays(-random.Next(0, 28));
            var types = new[] { NotificationType.Info, NotificationType.Success, NotificationType.Warning, NotificationType.Error };
            var titles = new[] { "New Application Received", "Candidate Qualified", "Interview Scheduled", "Feedback Submitted", "Offer Accepted", "Position Filled", "Application Rejected", "Reminder: Interview Tomorrow" };
            var title = titles[random.Next(titles.Length)];

            notifications.Add(new Notification
            {
                UserId = user.Id,
                Title = title,
                Message = $"Notification regarding {title.ToLower()}.",
                Type = types[random.Next(types.Length)],
                IsRead = random.NextDouble() > 0.3,
                ReferenceType = "Candidate",
                CreatedAt = createdAt
            });
        }
        context.Set<Notification>().AddRange(notifications);
        await context.SaveChangesAsync();

        // Email Templates
        var templates = new List<EmailTemplate>
        {
            new() { Name = "Application Received", Type = "ApplicationReceived", Subject = "Application Received - {CandidateName}", Body = "Dear {CandidateName},\n\nThank you for applying for the {JobTitle} position. We have received your application and will review it shortly.\n\nBest regards,\nRecruitment Team", IsActive = true },
            new() { Name = "Interview Invitation", Type = "InterviewInvitation", Subject = "Interview Invitation - {CandidateName}", Body = "Dear {CandidateName},\n\nWe are pleased to invite you for an interview for the {JobTitle} position. Please let us know your availability.\n\nBest regards,\nRecruitment Team", IsActive = true },
            new() { Name = "Rejection", Type = "Rejection", Subject = "Update on Your Application", Body = "Dear {CandidateName},\n\nThank you for your interest. After careful review, we have decided to move forward with other candidates.\n\nBest regards,\nRecruitment Team", IsActive = true },
            new() { Name = "Offer Letter", Type = "OfferLetter", Subject = "Offer Letter - {CandidateName}", Body = "Dear {CandidateName},\n\nWe are pleased to offer you the {JobTitle} position. Please review the attached offer.\n\nBest regards,\nRecruitment Team", IsActive = true },
            new() { Name = "Follow Up", Type = "FollowUp", Subject = "Following Up - {CandidateName}", Body = "Dear {CandidateName},\n\nWe are following up on your application for {JobTitle}. Please feel free to contact us with any questions.\n\nBest regards,\nRecruitment Team", IsActive = true }
        };
        context.Set<EmailTemplate>().AddRange(templates);
        await context.SaveChangesAsync();

        // Activities
        var activities = new List<Activity>();
        for (int i = 0; i < 100; i++)
        {
            var user = users[random.Next(users.Count)];
            var candidate = candidates[random.Next(candidates.Count)];
            var createdAt = candidate.CreatedAt.AddDays(random.Next(1, 60));
            activities.Add(new Activity
            {
                UserId = user.Id,
                CandidateId = candidate.Id,
                Type = (ActivityType)random.Next(Enum.GetValues<ActivityType>().Length),
                Description = $"Activity logged for {candidate.FullName}",
                CreatedAt = createdAt
            });
        }
        context.Set<Activity>().AddRange(activities);
        await context.SaveChangesAsync();
    }
}
