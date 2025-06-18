using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.MessageStatuses.DTOs
{
    public class AddMessageStatusDto
    {  
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public enMessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}