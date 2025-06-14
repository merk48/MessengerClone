using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.ChatMembers.DTOs
{
    public class AddChatMemberDto
    {
        public int UserId { get; set; }
        public enChatRole ChatRole { get; set; }
    }

}
