using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Users.DTOs;

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