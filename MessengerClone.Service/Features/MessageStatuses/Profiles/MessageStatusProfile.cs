using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.MessageStatuses.DTOs;

namespace MessengerClone.Service.Features.MessageStatuses.Profiles
{
    public class MessageStatusProfile : Profile
    {
        public MessageStatusProfile()
        {
            CreateMap<MessageStatus, MessageStatusDto>();
            //.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<AddMessageStatusDto, MessageStatus>();
                //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => enMessageStatus.Sent));

        }
    }
}
