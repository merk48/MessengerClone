namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ChangeEmailDto
    {
        public required string OldEmail { get; set; } 
        public required string NewEmail { get; set; } 
        public required string ConfirmedEmail { get; set; }
    } 
}
