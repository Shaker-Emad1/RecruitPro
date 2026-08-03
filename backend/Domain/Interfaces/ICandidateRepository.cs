using Recruitment.Domain.Entities;

namespace Recruitment.Domain.Interfaces;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<Candidate?> GetByEmailAsync(string email);
    Task<IEnumerable<Candidate>> SearchAsync(string term);
    Task<IEnumerable<Candidate>> GetByScoreRangeAsync(int min, int max);
    Task<int> GetTotalCountAsync();
    Task<int> GetQualifiedCountAsync();
    Task<double> GetConversionRateAsync();
}
