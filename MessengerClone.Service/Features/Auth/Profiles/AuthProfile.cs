using AutoMapper;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Auth.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore());
                
                //.ForMember(dest => dest.ProfileImageUrl,
                //opt => opt.MapFrom((src, dest, destMember, context) => (string)context.Items["ProfileImageUrl"]));

        }
    }
}
