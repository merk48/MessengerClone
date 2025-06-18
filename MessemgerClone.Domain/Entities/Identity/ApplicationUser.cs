using Microsoft.AspNetCore.Identity;
namespace MessengerClone.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ProfileImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public DateTime LastSeen { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public List<ChatMember>? ChatMembers { get; set; } = new();
        public List<GroupChat>? CreatedGroupConversations { get; set; } = new();
    }
}
