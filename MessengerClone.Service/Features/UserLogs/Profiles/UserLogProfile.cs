using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.UserLogs.DTOs;

namespace MessengerClone.Service.Features.UserLogs.Profiles
{
    public class UserLogProfile : Profile
    {
        public UserLogProfile()
        {

            CreateMap<AddLogUserDto, UserLog>();

               //.ForMember(dest => dest.UserId, opt => opt.MapFrom((src, dest, destMember, context) =>
               //                 (int)context.Items["UserId"]));
        }
    }
}
