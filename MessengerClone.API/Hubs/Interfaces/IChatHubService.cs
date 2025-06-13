using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MessengerClone.API.Hubs.Chathub.Interfaces
{
    public interface IChatHubService
    {
        Task<Result> JoinChatAsync(string connectionId, int userId, int chatId, CancellationToken cancellationToken);
        Task<Result> JoinAllChatsAsync(string connectionId, int userId, List<int> Ids);
        Task<Result> JoinAllChatsAsync(string connectionId, int userId, CancellationToken cancellationToken);
        Task<Result> LeaveChatAsync(string connectionId, int userId, int chatId, CancellationToken cancellationToken);
        Result LeaveAllChatsAsync(string connectionId, int userId, List<int> chatIds);
        Task<Result> LeaveAllChatsAsync(string connectionId, int userIds, CancellationToken cancellationToken);
        Task<Result> SendMessageAsync(AddMessageDto dto, int userId, int chatId, CancellationToken cancellationToken);
    }
}
