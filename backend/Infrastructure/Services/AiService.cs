using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiService> _logger;

    public AiService(HttpClient httpClient, IConfiguration configuration, ILogger<AiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(string Summary, int Score, string Classification, string Recommendations, string Strengths, string Weaknesses, string InterviewQuestions, string HiringRecommendation)> AnalyzeCandidateAsync(Candidate candidate, Job? job = null)
    {
        try
        {
            var prompt = BuildAnalysisPrompt(candidate, job);
            var response = await CallOpenAiAsync(prompt);
            var result = ParseAnalysisResponse(response);
            _logger.LogInformation("AI analysis completed for candidate {Id}", candidate.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI analysis failed for candidate {Id}, using fallback scoring", candidate.Id);
            return FallbackAnalysis(candidate, job);
        }
    }

    public async Task<string> GenerateEmailAsync(string type, Candidate candidate, Job? job = null)
    {
        try
        {
            var prompt = BuildEmailPrompt(type, candidate, job);
            return await CallOpenAiAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email generation failed for candidate {Id}", candidate.Id);
            return FallbackEmail(type, candidate);
        }
    }

    public async Task<int> CalculateMatchScoreAsync(Candidate candidate, Job job)
    {
        try
        {
            var prompt = $@"Calculate a match score (0-100) between a candidate and a job.

Candidate Skills: {string.Join(", ", candidate.Skills.Select(s => s.Name))}
Candidate Experience: {candidate.YearsOfExperience} years
Candidate Education: {candidate.Education}

Job Title: {job.Title}
Required Skills: {job.RequiredSkills}
Min Experience: {job.MinimumExperience} years

Return only a single integer score.";
            var response = await CallOpenAiAsync(prompt);
            if (int.TryParse(response.Trim(), out var score))
                return Math.Clamp(score, 0, 100);
            return 50;
        }
        catch
        {
            return CalculateFallbackScore(candidate, job);
        }
    }

    public async Task<string> ExtractResumeDataAsync(string resumeText)
    {
        try
        {
            var prompt = $@"Extract the following information from this resume text. Return as JSON with these fields: fullName, email, phone, skills (array), education, experience (years), certifications.

Resume Text:
{resumeText}";
            return await CallOpenAiAsync(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resume extraction failed");
            return "{}";
        }
    }

    private async Task<string> CallOpenAiAsync(string prompt)
    {
        var apiKey = _configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "sk-placeholder")
            throw new InvalidOperationException("OpenAI API key is not configured.");

        var model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";

        var requestBody = new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.3,
            max_tokens = 2000
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var response = await _httpClient.SendAsync(request, cts.Token);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var text = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return text?.Trim() ?? "";
    }

    private string BuildAnalysisPrompt(Candidate candidate, Job? job)
    {
        var skills = string.Join(", ", candidate.Skills.Select(s => $"{s.Name} ({s.ProficiencyLevel})"));
        var jobContext = job != null
            ? $"\nJob Title: {job.Title}\nJob Requirements: {job.Requirements}\nRequired Skills: {job.RequiredSkills}\nMin Experience: {job.MinimumExperience} years"
            : "";

        return $@"Analyze this candidate for a recruitment platform.

Candidate Name: {candidate.FullName}
Years of Experience: {candidate.YearsOfExperience}
Skills: {skills}
Education: {candidate.Education}
Certifications: {candidate.Certifications}
Location: {candidate.Location}
{jobContext}

Return a JSON object with exactly these fields:
- summary: A 2-3 sentence professional summary
- score: An integer from 0-100 representing overall fit
- classification: One of StrongMatch, ModerateMatch, or WeakMatch
- recommendations: 2-3 bullet points of recommendations
- strengths: 2-3 key strengths
- weaknesses: 2-3 areas for improvement
- interviewQuestions: 3 suggested interview questions
- hiringRecommendation: One of Highly Recommended, Recommended, Consider, or Not Recommended

Return ONLY valid JSON. No markdown, no code fences.";
    }

    private string BuildEmailPrompt(string type, Candidate candidate, Job? job)
    {
        var jobContext = job != null ? $" for the {job.Title} position in {job.Department}" : "";
        var typeDescriptions = new Dictionary<string, string>
        {
            ["ApplicationReceived"] = "A warm confirmation that their application was received",
            ["InterviewInvitation"] = "An invitation to schedule an interview",
            ["Rejection"] = "A polite rejection letter",
            ["OfferLetter"] = "A formal offer letter",
            ["FollowUp"] = "A friendly follow-up on their application status"
        };

        var description = typeDescriptions.GetValueOrDefault(type, "A professional email");

        return $@"Write a professional email {description}{jobContext}.

Candidate: {candidate.FullName}
Email: {candidate.Email}
Skills: {string.Join(", ", candidate.Skills.Select(s => s.Name))}

Write the email body only, no subject line. Be professional and warm.";
    }

    private (string, int, string, string, string, string, string, string) FallbackAnalysis(Candidate candidate, Job? job)
    {
        var score = candidate.YearsOfExperience * 10;
        score = Math.Clamp(score + (candidate.Skills.Count * 5), 0, 100);

        var classification = score >= 70 ? "StrongMatch" : score >= 40 ? "ModerateMatch" : "WeakMatch";
        var summary = $"{candidate.FullName} is a candidate with {candidate.YearsOfExperience} years of experience.";
        var recommendation = score >= 70 ? "Highly Recommended" : score >= 40 ? "Recommended" : "Consider";

        return (summary, score, classification, "Review candidate details.", "Relevant experience.", "Limited information available.", "What are your key achievements?", recommendation);
    }

    private int CalculateFallbackScore(Candidate candidate, Job job)
    {
        var score = 0;
        var candidateSkills = candidate.Skills.Select(s => s.Name.ToLower()).ToHashSet();
        var requiredSkills = job.RequiredSkills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => s.ToLower()).ToHashSet();

        if (requiredSkills.Count > 0)
        {
            var matchCount = requiredSkills.Count(s => candidateSkills.Contains(s));
            score += (int)((double)matchCount / requiredSkills.Count * 50);
        }

        if (candidate.YearsOfExperience >= job.MinimumExperience)
            score += 30;
        else
            score += (int)((double)candidate.YearsOfExperience / job.MinimumExperience * 30);

        score += Math.Min(candidate.Skills.Count, 20);

        return Math.Clamp(score, 0, 100);
    }

    private string FallbackEmail(string type, Candidate candidate)
    {
        return type switch
        {
            "ApplicationReceived" => $"Dear {candidate.FullName},\n\nThank you for your application. We have received your resume and will review it shortly. We will be in touch with next steps.\n\nBest regards,\nRecruitment Team",
            "InterviewInvitation" => $"Dear {candidate.FullName},\n\nWe were impressed by your application and would like to invite you for an interview. Please let us know your availability for the coming week.\n\nBest regards,\nRecruitment Team",
            "Rejection" => $"Dear {candidate.FullName},\n\nThank you for your interest in this position. After careful review, we have decided to move forward with other candidates whose qualifications more closely match our needs.\n\nWe wish you the best in your job search.\n\nBest regards,\nRecruitment Team",
            "OfferLetter" => $"Dear {candidate.FullName},\n\nWe are pleased to offer you the position. We were impressed by your skills and experience and believe you will be a great addition to our team.\n\nPlease review the attached offer letter and let us know your decision.\n\nBest regards,\nRecruitment Team",
            _ => $"Dear {candidate.FullName},\n\nWe are following up on your application status. Please feel free to reach out if you have any questions.\n\nBest regards,\nRecruitment Team"
        };
    }

    private (string, int, string, string, string, string, string, string) ParseAnalysisResponse(string response)
    {
        try
        {
            // Strip markdown code fences if present
            var cleaned = response.Replace("```json", "").Replace("```", "").Trim();
            using var doc = JsonDocument.Parse(cleaned);
            var root = doc.RootElement;

            var summary = root.GetProperty("summary").GetString() ?? "";
            var score = root.GetProperty("score").GetInt32();
            var classification = root.GetProperty("classification").GetString() ?? "ModerateMatch";
            var recommendations = root.GetProperty("recommendations").GetString() ?? "";
            var strengths = root.GetProperty("strengths").GetString() ?? "";
            var weaknesses = root.GetProperty("weaknesses").GetString() ?? "";
            var questions = root.GetProperty("interviewQuestions").GetString() ?? "";
            var hiringRec = root.GetProperty("hiringRecommendation").GetString() ?? "Consider";

            return (summary, Math.Clamp(score, 0, 100), classification, recommendations, strengths, weaknesses, questions, hiringRec);
        }
        catch
        {
            _logger.LogWarning("Failed to parse AI response, using fallback");
            return ("Candidate analysis is temporarily unavailable. A heuristic fallback was used.", 50, "ModerateMatch", "Review the candidate manually before proceeding.", "Profile data available for review.", "AI response could not be parsed reliably.", "What impact have you delivered in similar roles?", "Consider");
        }
    }
}
