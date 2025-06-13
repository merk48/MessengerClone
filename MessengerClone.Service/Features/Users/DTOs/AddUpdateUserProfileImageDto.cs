using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.Users.DTOs
{
    public class AddUpdateUserProfileImageDto
    {
        public IFormFile ProfileImage { get; set; } = null!;
    }
}
