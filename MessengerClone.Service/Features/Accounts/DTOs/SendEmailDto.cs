using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class SendEmailDto
    {
        public required string Email { get; set; } 
        public required enEmailType EmailType { get; set; } 
        public string? HtmlBody { get; set; }
    }
}
