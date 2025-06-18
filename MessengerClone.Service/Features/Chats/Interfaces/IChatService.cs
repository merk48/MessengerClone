using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using System.Linq.Expressions;

namespace MessengerClone.Service.Features.Chats.Interfaces
{
    public interface IChatService
    {
        Task<Result<ChatMetadataDto>> GetChatMetadataById(int chatId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<DataResult<ChatSidebarDto>>> GetAllForSidebarByUserIdAsync(int userId, CancellationToken cancellationToken, int? page = null, int? size = null,string? strFilter = null, Expression<Func<Chat, bool>>? filter = null);
        Task<Result<DataResult<int>>> GetUserAllChatIdsAsync(int userId, CancellationToken cancellationToken, int? page = null, int? size = null);
        Task<Result> IsChatExisits(int chatId, CancellationToken cancellationToken);
        Task<bool> IsGroupTitleTakenAsync(string title, CancellationToken cancellationToken);
        Task<bool> IsGroupTitleTakenAsync(string title, int chatId, CancellationToken cancellationToken);
        Task<Result<DirectChatMetadataDto>> AddDirectChatAsync(AddDirectChatDto dto, int createdByUserId, CancellationToken cancellationToken);
        Task<Result<GroupChatMetadataDto>> AddGroupChatAsync(AddGroupChatDto dto, int currentUserId, CancellationToken cancellationToken);
        Task<Result<GroupChatMetadataDto>> RenameGroupChatAsync(int chatId, int currentUserId, RenameGroupChatDto dto, CancellationToken cancellationToken);
        Task<Result<GroupChatMetadataDto>> ResetGroupChatImageAsync(int chatId, int currentUserId, ResetGroupChatImageDto dto, CancellationToken cancellationToken);
        Task<Result<GroupChatMetadataDto>> DeletetGroupChatImageAsync(int chatId,int currentUserId, CancellationToken cancellationToken);
        Task<Result<GroupChatMetadataDto>> UpdateGroupChatDescriptionAsync(int chatId, int currentUserId, UpdateGroupChatDescriptionDto dto, CancellationToken cancellationToken);
        Task<Result> UpdateGroupLastMessageAsync(int chatId, int currentUserId, MessageDto msgDto, CancellationToken cancellationToken);
        Task<Result> DeleteGroupChatAsync(int chatId, int currentUserId, CancellationToken cancellationToken);


    }

}
