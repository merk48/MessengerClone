using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.Messages.DTOs
{
    public class AddMessageDto
    {
        public string Content { get; set; } = null!;
        public enMessageType Type { get; set; }
        public AddAttachmentDto? AddAttachmentDto { get; set; }
    } 
}