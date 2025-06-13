using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MessageReaction : ICreateAt
    {
        public enMessageReactionType ReactionType { get; set; } 
        public DateTime CreatedAt { get; set; } // Reacted at
        
        // Navigation
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public int? MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
