using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;

namespace MessengerClone.Service.Features.Messages.Profiles
{
    class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Attachment, opt => opt.MapFrom(src => src.Attachment))
                .ForMember(dest => dest.MessageInfo, opt => opt.MapFrom(src => src.MessageInfo))
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.MessageReactions));

            CreateMap<LastMessageSnapshot, LastMessageDto>();

            CreateMap<Message, LastMessageDto>()
                .ForMember(dest => dest.SenderUserame, opt => opt.MapFrom(src => src.Sender.UserName))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<AddMessageDto, Message>()
                .ForMember(dest => dest.Attachment, opt => opt.Ignore());

        }
    }
}
