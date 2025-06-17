using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class MediaAttachment :ISoftDeletable
    {
        public int MessageId { get; set; } // pk and fk
        public string AttachmentUrl { get; set; } = null!;
        public enMediaType FileType { get; set; }
        public bool IsDeleted  {get; set; }
        public DateTime? DateDeleted  {get; set; }
        public int? DeletedBy  {get; set; }

        public Message Message { get; set; } = null!;
        public ApplicationUser? Deleter  {get; set; }
    }
}
