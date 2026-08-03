using Recruitment.Domain.Entities;

namespace Recruitment.Domain.Interfaces;

public interface IAiService
{
    Task<(string Summary, int Score, string Classification, string Recommendations, string Strengths, string Weaknesses, string InterviewQuestions, string HiringRecommendation)> AnalyzeCandidateAsync(Candidate candidate, Job? job = null);
    Task<string> GenerateEmailAsync(string type, Candidate candidate, Job? job = null);
    Task<int> CalculateMatchScoreAsync(Candidate candidate, Job job);
    Task<string> ExtractResumeDataAsync(string resumeText);
}
