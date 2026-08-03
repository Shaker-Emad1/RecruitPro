using AutoMapper;
using Recruitment.Application.DTOs;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Candidate, CandidateDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Classification, o => o.MapFrom(s => s.Classification.ToString()))
            .ForMember(d => d.Skills, o => o.MapFrom(s => s.Skills.Select(x => new SkillDto(x.Name, x.Category, x.ProficiencyLevel)).ToList()));
        CreateMap<CreateCandidateRequest, Candidate>()
            .ForMember(d => d.Skills, o => o.Ignore());

        CreateMap<Job, JobDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ApplicationCount, o => o.MapFrom(s => s.Applications.Count));
        CreateMap<CreateJobRequest, Job>();

        CreateMap<Recruitment.Domain.Entities.Application, ApplicationDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CandidateName, o => o.MapFrom(s => s.Candidate.FullName))
            .ForMember(d => d.CandidateEmail, o => o.MapFrom(s => s.Candidate.Email))
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Job.Title));

        CreateMap<Interview, InterviewDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CandidateName, o => o.MapFrom(s => s.Candidate.FullName))
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Job.Title))
            .ForMember(d => d.InterviewerName, o => o.MapFrom(s => s.Interviewer.FullName))
            .ForMember(d => d.Feedback, o => o.MapFrom(s => s.Feedback));

        CreateMap<InterviewFeedback, InterviewFeedbackDto>();
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));
        CreateMap<Activity, ActivityDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));

        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
    }
}
