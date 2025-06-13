using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.UserLogs.DTOs
{
    public class AddLogUserDto
    {
        public int UserId { get; set; } 
        public enUserLogEvent Event { get; set; }
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string? Message { get; set; }
    } 
}
