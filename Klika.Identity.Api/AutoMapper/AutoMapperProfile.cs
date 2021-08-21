using AutoMapper;
using Klika.Identity.Model.DTO.Request;
using Klika.Identity.Model.DTO.Response;
using Klika.Identity.Model.Entities;

namespace Klika.Identity.Api.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserRequestDTO>()
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.UserName))
                .ReverseMap();
            
            CreateMap<ApplicationUser, ApplicationUserResponseDTO>()
                .ForMember(dest => dest.IsEmailConfirmed, opts => opts.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<ApplicationUser, UserResponseDTO>()
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEmailConfirmed, opts => opts.MapFrom(src => src.EmailConfirmed))
                .ReverseMap();
        }
    }
}
