using Recruitment.Domain.Entities;

namespace Recruitment.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}
