using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ChatAdminUserDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? ProfileFileUrl { get; set; }
        public enChatRole Roles { get; set; }
    }
}
