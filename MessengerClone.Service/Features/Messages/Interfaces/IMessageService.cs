using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;
using System.Linq.Expressions;

namespace MessengerClone.Service.Features.Messages.Interfaces
{
    public interface IMessageService
    {
        Task<Result<MessageDto>> AddMessageAsync(AddMessageDto dto, int senderId,int chatId, CancellationToken cancellationToken);
        Task<Result<LastMessageDto>> GetLatestMessageInChatAsync(int chatId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<DataResult<MessageDto>>> GetChatMessagesForUserAsync(int chatId, int userId, CancellationToken cancellationToken, int? page, int? size,string? strFilter = null, Expression<Func<Message, bool>>? filter = null);
        Task<Result<MessageDto>> GetMessageByIdForUserAsync(int Id, int currentUserId, CancellationToken cancellationToken);
        Task<Result<MessageDto>> PinMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<MessageDto>> UnPinMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<MessageDto>> DeleteMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken);
        Task<Result<MessageDto>> UndoDeleteMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken);
    
    }
}
