using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class VerificationRequestDto
    {
        public enVerificationType Type { get; set; }
        //public required string OldValue { get; set; } 
        public required string NewValue { get; set; }  // email/phone or new password for reset
    } 
}
