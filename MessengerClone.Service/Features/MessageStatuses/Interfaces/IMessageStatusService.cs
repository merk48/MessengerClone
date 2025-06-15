using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.MessageStatuses.DTOs;

namespace MessengerClone.Service.Features.MessageStatuses.Interfaces
{
    public interface IMessageStatusService
    {
        Task<Result> MarkAsDeliveredAsync(int messageId, int userId);
        Task<Result> MarkAsReadAsync(int messageId, int userId);
        Task<Result<DataResult<MessageStatusDto>>> GetStatusesForMessageAsync(int messageId);
        Task<Result<int>> GetChatUnreadMessagesCountForUserAsync(int chatId, int currentUserId);
        Task<Result<DataResult<MessageDto>>> GetChatUnreadMessagesForUserAsync(int chatId, int currentUserId, int? page = null, int? size = null);
    }
}
