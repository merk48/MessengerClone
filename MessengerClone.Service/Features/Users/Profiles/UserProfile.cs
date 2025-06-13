using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Users.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                    .ForMember(dest => dest.Roles, opt => opt.MapFrom((src, dest, destMember, context) =>
                                (List<string>)context.Items["Roles"]))
                    .ForMember(dest => dest.locked,
                           opt => opt.MapFrom((src, dest, destMember, context) =>
                                (bool)context.Items["IsLocked"]));

            CreateMap<ApplicationUser, ChatMemberDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ProfileFileUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.JoinedAt,
                           opt => opt.MapFrom((src, dest, destMember, context) => {
                               var joinedAt = context.Items["JoinedAt"];
                               return joinedAt;
                           }));

            CreateMap<UpdateUserDto, ApplicationUser>()
               .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
               .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore());

        }
    }
}
