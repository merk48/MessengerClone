using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.ChatMembers.Profiles
{
    class ChatMemberProfile : Profile
    {
        public ChatMemberProfile()
        {
            CreateMap<ChatMember, ChatMemberDto>()
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                 .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                 .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                 .ForMember(dest => dest.ProfileFileUrl, opt => opt.MapFrom(src => src.User.ProfileImageUrl))
                 .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<AddChatMemberDto, ChatMember>()
                 .ForMember(dest => dest.ChatId, opt => opt.MapFrom((src, dest, destMember, context) => (int)context.Items["ChatId"]));

        }
    }
}
