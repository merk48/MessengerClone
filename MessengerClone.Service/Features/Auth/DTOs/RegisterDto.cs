using MessengerClone.Domain.Utils.Enums;
using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.Auth.DTOs
{
    public class RegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        //[UniqueUser]
        public required string Email { get; set; }
        public required string Password { get; set; }
        public List<enAppUserRoles> Roles { get; set; } = null!;
    }
}
