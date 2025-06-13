namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ChangePhoneNumberDto
    {
        public required string OldPhoneNumber { get; set; }
        public required string NewPhoneNumber { get; set; }
        public required string ConfirmedPhoneNumber { get; set; }
    }
}
