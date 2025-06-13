using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.DTOs
{
    public class LastMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public string SenderUserame { get; set; } = null!;
        public enMessageType Type { get; set; } 
    }
}
