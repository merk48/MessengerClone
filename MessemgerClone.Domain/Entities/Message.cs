using MessengerClone.Domain.Common;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class Message : BaseEntity, ICreateAt, ISoftDeletable
    { // no update for now
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public enMessageType Type { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedById { get; set; }
        public ApplicationUser? PinnedByUser { get; set; }

        public DateTime CreatedAt { get; set; } // sendAt
        public int SenderId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int? DeletedById { get; set; }

        // Navigation
        public Chat Chat { get; set; } = null!;

        public ApplicationUser Sender { get; set; } = null!;
        public ApplicationUser? DeletedBy { get; set; }

        public MediaAttachment? Attachment { get; set; }

        public List<MessageStatus> MessageStatuses { get; set; } = new();
        public List<MessageReaction> MessageReactions { get; set; } = new();


        public void UndoDelete()
        {
            IsDeleted = false;
            DateDeleted = null;
            DeletedById = null;

            if(Attachment is not null)
            {
                Attachment.IsDeleted = false;
                Attachment.DateDeleted = null;
                Attachment.DeletedById = null;
            }
        }
    }

}
