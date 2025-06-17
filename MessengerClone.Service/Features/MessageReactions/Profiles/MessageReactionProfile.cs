using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.MessageReactions.DTOs;

namespace MessengerClone.Service.Features.MessageReactions.Profiles
{
    public class MessageReactionProfile : Profile
    {
        public MessageReactionProfile()
        {
            CreateMap<MessageReaction, MessageReactionDto>()
                 .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Member));

            CreateMap<AddMessageReactionDto, MessageReaction>()
                .ForMember(dest => dest.UserId,opt => opt.MapFrom((src, dest, destMember, context) => (int)context.Items["CurrentUserId"]))
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom((src, dest, destMember, context) => (int)context.Items["MessageId"]));
        }
    }

}
