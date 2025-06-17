using MessengerClone.Domain.Common;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class Message : BaseEntity, ICreateAt, ISoftDeletable
    { 
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public enMessageType Type { get; set; }
        public DateTime CreatedAt { get; set; } // sendAt
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public MediaAttachment? Attachment { get; set; }

        // Navigation
        public Chat Chat { get; set; } = null!;
        public ChatMember Sender { get; set; } = null!;
        public ChatMember? Deleter { get; set; }
        public ChatMember? PinnedByMember { get; set; }
        public List<MessageStatus> MessageStatuses { get; set; } = new();
        public List<MessageReaction> MessageReactions { get; set; } = new();


        public void UndoDelete()
        {
            IsDeleted = false;
            DateDeleted = null;
            DeletedBy = null;

            if(Attachment is not null)
            {
                Attachment.IsDeleted = false;
                Attachment.DateDeleted = null;
                Attachment.Deleter = null;
            }
        }
    }

}
