using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;

namespace MessengerClone.Service.Features.MessageReactions.Interfaces
{
    public interface IMessageReactionService
    {
        Task<Result<MessageReactionDto>> AddReactToMessageAsync(int messageId, int currentUserId, AddMessageReactionDto dto);
        Task<Result> RemoveReactionToMessageAsync(int messageId ,int currentUserId);
    }
}
