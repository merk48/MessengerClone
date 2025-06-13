using MessengerClone.Service.Features.ChatMembers.DTOs;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    public class DirectChatMetadataDto : ChatMetadataDto
    {
        public ChatMemberDto OtherUser { get; set; } = null!;
    }
}