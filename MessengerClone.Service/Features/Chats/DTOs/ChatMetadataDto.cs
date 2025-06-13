using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.DTOs;
using Newtonsoft.Json;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.Auto)]
    //[JsonConverter(typeof(JsonSubtypes), "Type")]
    //[JsonSubtypes.KnownSubType(typeof(GroupChatMetadataDto), enChatType.Group)]
    //[JsonSubtypes.KnownSubType(typeof(DirectChatMetadataDto), enChatType.Direct)]
    public class ChatMetadataDto
    {
        public int Id { get; set; }
        public enChatType Type { get; set; }
        public int UnreadCount { get; set; } = 0;
        public LastMessageDto? LastMessage { get; set; } = new();
        public List<ChatMemberDto> Members { get; set; } = new();
    }
}