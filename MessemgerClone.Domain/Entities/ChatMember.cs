using MessengerClone.Domain.Common;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class ChatMember : BaseEntity, ICreateAt 
    {
        public DateTime CreatedAt { get; set; }
        public enChatRole ChatRole { get; set; }

        // Navigation
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public List<MessageReaction>? MessageReactions { get; set; } = new();
        public List<MessageStatus>? MessageInfo { get; set; } = new();

    }
}
