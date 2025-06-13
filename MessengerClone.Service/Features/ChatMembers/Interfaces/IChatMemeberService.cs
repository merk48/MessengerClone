using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.General.DTOs;

namespace MessengerClone.Service.Features.Chats.Interfaces
{
    public interface IChatMemeberService
    {
        Task<Result<ChatMemberDto>> AddMemberToChatAsync(AddChatMemberDto dto);
        Task<Result<DataResult<ChatMemberDto>>> AddRangeOfMembersToChatAsync(IEnumerable<AddChatMemberDto> dto);
        Task<Result> RemoveMemberFromChatAsync(int userId, int chatId);
        Task<Result<ChatMemberDto>> GetMemberInChatAsync(int userId, int chatId);
        Task<Result<ChatMemberDto>> ChangeMemberChatRoleAsync(int userId, int chatId, AddMemberChatRoleDto dto);
        Task<Result> IsUserMemberInChat(int chatId, int currentUserId);
    }
}
