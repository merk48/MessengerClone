using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.DTOs;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    // This for the sidebar of the application 
    public class ChatSidebarDto
    {
        public int Id { get; set; }
        public enChatType Type { get; set; }
        public LastMessageDto? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public string DisplayName { get; set; } = null!;
        public string DisplayFile { get; set; } = null!;
        public string? Description { get; set; }
    }
}