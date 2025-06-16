using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;

namespace MessengerClone.Service.Features.MessageStatuses.DTOs
{
    public class MessageStatusDto
    { 
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public enMessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; } 
        public ChatMemberDto User { get; set; } = null!;
    }
}
