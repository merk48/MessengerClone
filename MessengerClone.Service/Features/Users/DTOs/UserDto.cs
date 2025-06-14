using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public bool locked { get; set; }
        public List<enAppUserRoles> Roles { get; set; } = null!;
    }
}
