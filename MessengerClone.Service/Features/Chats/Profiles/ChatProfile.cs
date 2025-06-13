using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Chats.DTOs;

namespace MessengerClone.Service.Features.Chats.Profiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {

            CreateMap<Chat, ChatSidebarDto>()
                .Include<GroupChat, ChatSidebarDto>()
                .Include<DirectChat, ChatSidebarDto>();

            CreateMap<GroupChat, ChatSidebarDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => enChatType.Group))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.DisplayFile,opt => opt.MapFrom(src => src.GroupCoverImageUrl))
                .ForMember(dest => dest.Description,  opt => opt.MapFrom(src => src.Description));

            CreateMap<DirectChat, ChatSidebarDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => enChatType.Direct))
                .ForMember(dest => dest.DisplayName,
                           opt => opt.MapFrom((src, dest, destMember, context) =>
                           {
                               var currentUserId = (int)context.Items["CurrentUserId"];
                               var other = src.ChatMembers
                                   .FirstOrDefault(cm => cm.UserId != currentUserId);
                               return other != null
                                   ? other.User.UserName
                                   : string.Empty;
                           }))
                .ForMember(dest => dest.DisplayFile,
                           opt => opt.MapFrom((src, dest, destMember, context) =>
                           {
                               var currentUserId = (int)context.Items["CurrentUserId"];
                               var other = src.ChatMembers
                                   .FirstOrDefault(cm => cm.UserId != currentUserId);
                               return other != null
                                   ? other.User.ProfileImageUrl
                                   : null;
                           }))
                .ForMember(dest => dest.Description, opt => opt.Ignore());



            CreateMap<Chat, ChatMetadataDto>()
                 .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.ChatMembers))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.LastMessage))
                .ForMember(dest => dest.UnreadCount, opt => 
                                                    opt.MapFrom((src, dest, destMember, context) => (int)context.Items["UnreadCount"]))
             .Include<GroupChat, GroupChatMetadataDto>()
             .Include<DirectChat, DirectChatMetadataDto>();
            
            CreateMap<DirectChat, DirectChatMetadataDto>()
                .ForMember(dest => dest.OtherUser,
                           opt => opt.MapFrom((src, dest, destMember, context) =>
                           {
                               var currentUserId = (int)context.Items["CurrentUserId"];
                               return src.ChatMembers.FirstOrDefault(cm => cm.UserId != currentUserId);
                           }));

            CreateMap<GroupChat, GroupChatMetadataDto>();



            CreateMap<AddDirectChatDto, DirectChat>()
                   .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => enChatType.Direct));

            CreateMap<AddGroupChatDto, GroupChat>()
                .ForMember(dest => dest.GroupCoverImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                   .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => enChatType.Group));

        }
    }
}
