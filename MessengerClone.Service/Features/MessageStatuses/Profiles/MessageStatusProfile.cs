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
            .ForMember(dest => dest.Member, opt => opt.MapFrom(src => src.Member));

            CreateMap<AddMessageStatusDto, MessageStatus>();
                //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => enMessageStatus.Sent));

        }
    }
}
