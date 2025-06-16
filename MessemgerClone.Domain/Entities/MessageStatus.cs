using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MessageStatus : ICreateAt
    {
        // Composite Key: MessageId + UserId
        public int MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public int MemberId { get; set; }
        public ChatMember Member { get; set; } = null!;

        public enMessageStatus Status { get; set; }
        //public enMessageStatus Status => ReadAt != null ? enMessageStatus.Read : DeliveredAt != null ? enMessageStatus.Delivered : enMessageStatus.Sent;

        public DateTime? DeliveredAt { get; set; } // (Arrived/Delivered)

        public DateTime? ReadAt { get; set; } // (Seen/Read)

        public DateTime CreatedAt { get; set; } // (Sent) = Message.CreatedAt
    }
}
