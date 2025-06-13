using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.MessageStatuses.DTOs;

namespace MessengerClone.Service.Features.MessageStatuses.Profiles
{
    public class MessageStatusProfile : Profile
    {
        public MessageStatusProfile()
        {
            CreateMap<MessageStatus, MessageStatusDto>()
                 .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.User));

            CreateMap<CreateMessageStatusDto, MessageStatus>();
        }
    }
}
