using MessengerClone.Domain.Common;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Domain.Entities
{
    public class Chat : BaseEntity, ICreateAt, ISoftDeletable
    {
        public enChatType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public enChatTheme ChatTheme { get; set; }
        public LastMessageSnapshot? LastMessage { get; set; } 
        public string? Title { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public ChatMember? Deleter { get; set; }

        //Navigation
        public List<ChatMember> ChatMembers { get; set; } = new();
        public List<Message>? Messages { get; set; } = new();
    }
}
