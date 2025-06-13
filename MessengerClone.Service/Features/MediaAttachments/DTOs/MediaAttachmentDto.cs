using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.MediaAttachments.DTOs
{
    public class MediaAttachmentDto
    {
        public string AttachmentUrl { get; set; } = null!;
        public enMediaType FileType { get; set; }
    }
}
