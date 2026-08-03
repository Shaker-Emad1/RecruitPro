using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Recruitment.API.Middleware;
using Recruitment.Application.Interfaces;
using Recruitment.Application.Mappings;
using Recruitment.Application.Services;
using Recruitment.Application.Validators;
using Recruitment.Domain.Interfaces;
using Recruitment.Infrastructure.Data;
using Recruitment.Infrastructure.Repositories;
using Recruitment.Infrastructure.Seed;
using Recruitment.Infrastructure.Services;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/recruitment-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "RecruitmentPlatformSecretKey2024!@#$%^&*()";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RecruitmentPlatform";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RecruitmentPlatformClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("HRManagerOrAbove", p => p.RequireRole("Admin", "HRManager"));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

builder.Services.AddDbContext<RecruitmentDbContext>(options =>
{
    var connString = builder.Configuration.GetConnectionString("SqliteConnection")
        ?? "Data Source=recruitment.db";
    options.UseSqlite(connString);
});

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<RecruitmentDbContext>());
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IAiService, AiService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Recruitment Automation API",
        Version = "v1",
        Description = "Production-ready AI-powered recruitment automation platform"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RecruitmentDbContext>();
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        await DataSeeder.SeedAsync(dbContext);
        Log.Information("Database seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database seeding skipped (may already be seeded or SQL Server not available)");
    }
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseRateLimiter();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Recruitment Automation API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
