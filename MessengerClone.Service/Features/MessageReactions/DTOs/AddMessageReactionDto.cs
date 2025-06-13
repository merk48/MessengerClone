using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.MessageReactions.DTOs
{
    public class AddMessageReactionDto
    {
        public enMessageReactionType ReactionType { get; set; }
    }
}