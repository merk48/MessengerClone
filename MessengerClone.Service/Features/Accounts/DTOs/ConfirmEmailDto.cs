using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ConfirmEmailDto
    {
        public required string UserId { get; set; } 
        public required string ConfirmToken { get; set; } 
        public required enEmailType EmailType { get; set; }
    }
}
