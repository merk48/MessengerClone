using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using System.Linq.Expressions;

namespace MessengerClone.Service.Features.Chats.Interfaces
{
    public interface IChatMemeberService
    {
        Task<Result<DataResult<ChatMemberDto>>> GetAllChatMembersAsync(int chatId, CancellationToken cancellationToken, int? page = null, int? size = null, string? strFilter = null, Expression<Func<ChatMember, bool>>? filter = null); Task<Result<ChatMemberDto>> AddMemberToChatAsync(AddChatMemberDto dto, int chatId);
        Task<Result<ChatMemberDto>> GetMemberInChatAsync(int userId, int chatId);
        Task<Result<DataResult<ChatMemberDto>>> AddRangeOfMembersToChatAsync(IEnumerable<AddChatMemberDto> dto, int chatId);
        Task<Result> RemoveMemberFromChatAsync(int userId, int chatId);
        Task<Result<ChatMemberDto>> ChangeMemberChatRoleAsync(int userId, int chatId, AddMemberChatRoleDto dto);
        Task<Result<bool>> IsUserMemberInChatAsync(int chatId, int currentUserId);
    }
}
