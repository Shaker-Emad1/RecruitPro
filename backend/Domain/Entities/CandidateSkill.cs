namespace Recruitment.Domain.Entities;

public class CandidateSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;

    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}
