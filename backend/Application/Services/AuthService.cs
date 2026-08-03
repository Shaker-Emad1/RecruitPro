using AutoMapper;
using Microsoft.Extensions.Logging;
using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces;
using Recruitment.Domain.Entities;
using Recruitment.Domain.Enums;
using Recruitment.Domain.Interfaces;

namespace Recruitment.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email.ToLower());
        if (existing.Any())
            return new ApiResponse<AuthResponse>(false, "Email already registered", null);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Enum.Parse<UserRole>(request.Role)
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        _logger.LogInformation("User {Email} registered as {Role}", user.Email, user.Role);

        return new ApiResponse<AuthResponse>(true, "Registration successful",
            new AuthResponse(token, _mapper.Map<UserDto>(user)));
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email.ToLower());
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new ApiResponse<AuthResponse>(false, "Invalid email or password", null);

        if (!user.IsActive)
            return new ApiResponse<AuthResponse>(false, "Account is deactivated", null);

        var token = _jwtService.GenerateToken(user);
        _logger.LogInformation("User {Email} logged in", user.Email);

        return new ApiResponse<AuthResponse>(true, "Login successful",
            new AuthResponse(token, _mapper.Map<UserDto>(user)));
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return new ApiResponse<UserDto>(false, "User not found", null);

        return new ApiResponse<UserDto>(true, "OK", _mapper.Map<UserDto>(user));
    }
}
