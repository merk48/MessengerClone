namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ConfirmRequestDto : VerificationRequestDto
    {
        public required string Token { get; set; }
    }
}
