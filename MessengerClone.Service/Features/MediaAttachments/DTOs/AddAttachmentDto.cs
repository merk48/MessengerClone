using MessengerClone.Domain.Utils.Enums;
using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.MediaAttachments.DTOs
{
    public class AddAttachmentDto
    {
        public IFormFile? Attachment { get; set; } = null!;
        public enMediaType? FileType { get; set; }
    }
}