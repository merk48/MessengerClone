using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MessageStatus : ICreateAt
    {
        // Composite Key: MessageId + UserId + ChatId
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public enMessageStatus Status { get; set; }
        public DateTime? DeliveredAt { get; set; } // (Arrived/Delivered)
        public DateTime? ReadAt { get; set; } // (Seen/Read)
        public DateTime CreatedAt { get; set; } // (Sent) = Message.CreatedAt
      
        // Navigation
        public Message Message { get; set; } = null!;
        public ChatMember Member { get; set; } = null!;
    }
}
