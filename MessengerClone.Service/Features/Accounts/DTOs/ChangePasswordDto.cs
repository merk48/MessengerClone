namespace MessengerClone.Service.Features.Users.DTOs
{
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmedPassword { get; set; }
    } 

    public class ResetForgetPasswordDto
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmedPassword { get; set; }
        //public required string PhoneNumber { get; set; } // later handle this
    }
}
