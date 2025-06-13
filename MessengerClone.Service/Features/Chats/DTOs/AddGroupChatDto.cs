using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MessengerClone.Service.Features.Chats.DTOs
{
    public class AddGroupChatDto
    {
        //[Required]
        //[MinLength(1, ErrorMessage = "MemberIds must contain at least one user.")]
        public int[] MemberIds { get; set; } = null!;

        //[UniqueGroupChatTitle]
        public string Title { get; set; } = null!;
        public IFormFile? GroupCoverImage { get; set; } 
        public string? Description { get; set; }
    }
}