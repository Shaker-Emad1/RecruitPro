using Microsoft.EntityFrameworkCore;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Infrastructure.Repositories;

public class CandidateRepository : Repository<Candidate>, ICandidateRepository
{
    public CandidateRepository(DbContext context) : base(context) { }

    public async Task<Candidate?> GetByEmailAsync(string email) =>
        await _dbSet.Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Email == email);

    public async Task<IEnumerable<Candidate>> SearchAsync(string term) =>
        await _dbSet.Include(c => c.Skills)
            .Where(c => c.FullName.ToLower().Contains(term) ||
                        c.Email.ToLower().Contains(term) ||
                        c.Skills.Any(s => s.Name.ToLower().Contains(term)))
            .ToListAsync();

    public async Task<IEnumerable<Candidate>> GetByScoreRangeAsync(int min, int max) =>
        await _dbSet.Where(c => c.AiScore >= min && c.AiScore <= max).ToListAsync();

    public async Task<int> GetTotalCountAsync() => await _dbSet.CountAsync();

    public async Task<int> GetQualifiedCountAsync() =>
        await _dbSet.CountAsync(c => c.AiScore >= 70);

    public async Task<double> GetConversionRateAsync()
    {
        var total = await _dbSet.CountAsync();
        if (total == 0) return 0;
        var hired = await _dbSet.CountAsync(c =>
            c.Status == Domain.Enums.CandidateStatus.Hired);
        return Math.Round((double)hired / total * 100, 2);
    }

    public override async Task<Candidate?> GetByIdAsync(Guid id) =>
        await _dbSet.Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id);
}
