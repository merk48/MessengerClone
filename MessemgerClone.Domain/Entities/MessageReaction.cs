using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MessageReaction : ICreateAt, ISoftDeletable
    {
        // Composite Key: MessageId + UserId + ChatId
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public enMessageReactionType ReactionType { get; set; } 
        public DateTime CreatedAt { get; set; } // Reacted at
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public ApplicationUser? Deleter { get; set; }

        // Navigation
        public ChatMember Member { get; set; } = null!;
        public Message? Message { get; set; }
    }
}
