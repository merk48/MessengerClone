using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;

namespace MessengerClone.Service.Features.MessageReactions.DTOs
{
    public class MessageReactionDto
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public enMessageReactionType ReactionType { get; set; }
        public DateTime CreatedAt { get; set; } // Reacted at
        public ChatMemberDto Member { get; set; } = null!;
    }
}
