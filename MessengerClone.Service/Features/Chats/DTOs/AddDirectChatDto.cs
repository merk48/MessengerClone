using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    public class AddDirectChatDto
    {
        public int OtherMemberId { get; set; }
    }
}