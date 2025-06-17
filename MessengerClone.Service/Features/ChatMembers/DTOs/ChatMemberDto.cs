using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.ChatMembers.DTOs
{
    public class ChatMemberDto
    {
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; } // deal with this cause it not loaded right from AppUser
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? ProfileFileUrl { get; set; }
        public enChatRole ChatRole { get; set; }
    }
}
