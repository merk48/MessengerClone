using AutoMapper;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Auth.DTOs;

namespace MessengerClone.Service.Features.Account.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
