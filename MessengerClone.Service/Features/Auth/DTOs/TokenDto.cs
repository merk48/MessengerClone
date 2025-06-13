namespace MessengerClone.Service.Features.Auth.DTOs
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiredTime { get; set; } 

    }
}
