using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;
using MessengerClone.Service.Features.MessageStatuses.DTOs;

namespace MessengerClone.Service.Features.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public enMessageType Type { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPinned { get; set; }
        public DateTime SentAt { get; set; }
        public MediaAttachmentDto? Attachment { get; set; }
        public ChatMemberDto Sender { get; set; } = null!;
        public List<MessageStatusDto> Statuses { get; set; } = new();
        public List<MessageReactionDto>? Reactions { get; set; }
    }
}
