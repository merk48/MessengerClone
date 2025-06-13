using JsonSubTypes;
using MessengerClone.Domain.Utils.Enums;
using Newtonsoft.Json;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    public class GroupChatMetadataDto : ChatMetadataDto
    {
        public string Title { get; set; } = null!;
        public string? GroupCoverImageUrl { get; set; }
        public string? Description { get; set; }
    }
}