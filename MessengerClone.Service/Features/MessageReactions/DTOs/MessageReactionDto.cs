using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;

namespace MessengerClone.Service.Features.MessageReactions.DTOs
{
    public class MessageReactionDto
    {
        public enMessageReactionType ReactionType { get; set; }
        public DateTime CreatedAt { get; set; } // Reacted at
        public int? MessageId { get; set; }
        public int UserId { get; set; }
        public ChatMemberDto User { get; set; } = null!;
    }
}
