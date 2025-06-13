using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    public class ResetGroupChatImageDto
    {
        public IFormFile NewImage { get; set; } = null!;
    }

}
