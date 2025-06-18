using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;

namespace MessengerClone.Service.Features.MessageReactions.Interfaces
{
    public interface IMessageReactionService
    {
        Task<Result<MessageReactionDto>> AddReactToMessageAsync(int chatId, int messageId, int currentUserId, AddMessageReactionDto dto);
        Task<Result<DataResult<MessageReactionDto>>> GetAllMessageReactionsAsync(int chatId, int messageId, int currentUserId);
        Task<Result> RemoveReactionToMessageAsync(int chatId, int messageId ,int currentUserId);
    }
}
