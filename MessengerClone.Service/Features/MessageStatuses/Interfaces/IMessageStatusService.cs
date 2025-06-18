using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.MessageStatuses.DTOs;

namespace MessengerClone.Service.Features.MessageStatuses.Interfaces
{
    public interface IMessageStatusService
    {
        Task<Result<MessageStatusDto>> AddMessageStatusAsync(AddMessageStatusDto dto);
        Task<Result<DataResult<MessageStatusDto>>> AddMessageInfoAsync(IEnumerable<AddMessageStatusDto> dto);
        Task<Result> MarkAsDeliveredAsync(int chatId, int messageId, int userId);
        Task<Result> MarkAsReadAsync(int chatId, int messageId, int userId);
        Task<Result<DataResult<MessageStatusDto>>> GetStatusesForMessageAsync(int chatId, int messageId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<int>> GetChatUnreadMessagesCountForUserAsync(int chatId, int currentUserId);
        Task<Result<DataResult<MessageDto>>> GetChatUnreadMessagesForUserAsync(int chatId, int currentUserId, CancellationToken cancellationToken, int? page = null, int? size = null);
    }
}
