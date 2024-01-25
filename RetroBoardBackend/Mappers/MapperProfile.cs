using AutoMapper;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Mappers.Actions;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserResponse>();

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Username)))
                .ForMember(dest => dest.Image, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Image)));

            CreateMap<RetrospectiveDto, Retrospective>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false));

            CreateMap<Retrospective, RetrospectiveResponse>();

            CreateMap<UpdateRetrospectiveDto, Retrospective>()
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive || !src.IsActive));

            CreateMap<Retrospective, RetrospectiveResponseWithStats>();

            CreateMap<EntryDto, Entry>();

            CreateMap<Entry, EntryResponse>();

            CreateMap<Category, CategoryResponse>();

            CreateMap<EntryReactionDto, EntryReaction>();

            CreateMap<EntryReaction, EntryReactionResponse>();

            CreateMap<Category, CategoryWithRatio>();

            CreateMap<ProjectDto, Project>()
                .ForMember(src => src.IsDeleted, opt => opt.MapFrom(x => false))
                .AfterMap<SetProjectAction>();

            CreateMap<Project, PostProjectResponse>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(x => x.Categories))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(x => x.Users));

            CreateMap<UpdateProjectDto, Project>()
              .ForMember(dest => dest.Image, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Image)))
              .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Description)));

            CreateMap<Project, ProjectResponse>()
                .ForMember(dest => dest.PmUser, opt => opt.MapFrom(src => src.AuthorUser))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

            CreateMap<Project, MyProjectResponse>()
                .ForMember(dest => dest.AuthorUser, opt => opt.MapFrom(x => x.AuthorUser))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(x => x.Users.Count));
        }
    }
}
