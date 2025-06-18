using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MessageReaction : ICreateAt
    {
        // Composite Key: MessageId + UserId + ChatId
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public enMessageReactionType ReactionType { get; set; } 
        public DateTime CreatedAt { get; set; } // Reacted at
        
        // Navigation
        public ChatMember Member { get; set; } = null!;
        public Message? Message { get; set; }
    }
}
