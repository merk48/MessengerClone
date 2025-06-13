using MessengerClone.API.Hubs.Chathub.Interfaces;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.Users.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;


namespace MessengerClone.API.Hubs.Implementations
{

    public class ChatHubService
        (IHubContext<ChatHub, IChatClient> _hubContext , IUnitOfWork _unitOfWork, IMessageService _messageService, 
        IUserService _userService,IChatService _chatService , IChatMemeberService _chatMemeberService) 
        : IChatHubService
    {
        //                                 userId (connectionId, chatId)
        //private static ConcurrentDictionary<int, (string, int)> _userConnections = new();
        private static readonly ConcurrentDictionary<int, List<(string ConnectionId, int ChatId)>> _userConnections = new();

        private string chatName(int chatId) => $"chat-{chatId}";

        public async Task<Result> JoinChatAsync(string connectionId, int userId, int chatId, CancellationToken cancellationToken)
        {
            if (_userConnections[userId].Any(x => x.ConnectionId == connectionId && x.ChatId == chatId))
                return Result.Failure("User is already in chat");

            if (_userConnections.TryAdd(userId, new()))
                _userConnections[userId].Add((connectionId, chatId));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var addMemberResult = await _chatMemeberService.AddMemberToChatAsync(new AddChatMemberDto() {UserId = userId, ChatId = chatId });

                if(!addMemberResult.Succeeded)
                    return Result.Failure($"{addMemberResult.ToString()}");


                var result = await _userService.GetUserByIdAsync(userId, cancellationToken);

               if (result.Succeeded)
                {

                    var addMessageResult = await _messageService.AddMessageAsync(
                        new AddMessageDto()
                        { 
                            Content = $"{result.Data.Username} has joined this chat!",
                            Type = enMessageType.UserJoined,
                        }, userId, chatId);

                    if(!addMessageResult.Succeeded)
                        return Result.Failure($"{result.ToString()}");

                    await _hubContext.Clients.Group(chatName(chatId)).ReceiveLineMessage(result.Data!);

                    await _hubContext.Groups.AddToGroupAsync(connectionId, chatName(chatId));

                    return Result.Success();
               }
                else
                    return Result.Failure($"{result.ToString()}");
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result> JoinAllChatsAsync(string connectionId, int userId, CancellationToken cancellationToken)
        {
            try
            {
                
                var result = await _chatService.GetUserAllChatIdsAsync(userId, cancellationToken);

                if(!result.Succeeded)
                    return Result.Failure("Error retrieving chats for user.");


                var chatIds = result.Data?.Data.ToList();

                if (chatIds == null)
                    return Result.Failure("Error retrieving chats for user.");


                foreach (var chatId in chatIds)
                {
                    if (!_userConnections.TryGetValue(userId, out _))
                        _userConnections[userId] = new List<(string ConnectionId, int ChatId)>();

                    _userConnections[userId].Add((connectionId, chatId));

                    await _hubContext.Groups.AddToGroupAsync(connectionId, chatName(chatId));
                }

                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result> JoinAllChatsAsync(string connectionId, int userId, List<int> chatIds)
        {
            try
            {
                if (chatIds == null || !chatIds.Any())
                    return Result.Failure("User is not a member of any chat.");


                foreach (var chatId in chatIds)
                {
                    if (_userConnections.TryGetValue(userId, out _))
                        return Result.Failure("User is already in chat");

                    _userConnections[userId].Add((connectionId, chatId));

                    await _hubContext.Groups.AddToGroupAsync(connectionId, chatName(chatId));
                }

                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result> LeaveChatAsync(string connectionId, int userId, int chatId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                _userConnections[userId].Remove((connectionId,chatId));

                var deleteMemberResult = await _chatMemeberService.RemoveMemberFromChatAsync(userId, chatId );

                if (!deleteMemberResult.Succeeded)
                    return Result.Failure($"{deleteMemberResult.ToString()}");

                var result = await _userService.GetUserByIdAsync(userId, cancellationToken);

                if (result.Succeeded)
                {
                    var addMessageResult = await _messageService.AddMessageAsync(
                       new AddMessageDto()
                       {
                           Content = $"{result.Data.Username} has left this chat!",
                           Type = enMessageType.UserLeft,
                       }, userId, chatId);

                    if (!addMessageResult.Succeeded)
                        return Result.Failure($"{result.ToString()}");

                    await _hubContext.Clients.Group(chatName(chatId)).ReceiveLineMessage(result.Data!);

                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, chatName(chatId));

                    return Result.Success();
                }
                else
                    return Result.Failure("Failed to leave the chat.");

            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result> LeaveAllChatsAsync(string connectionId, int userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _chatService.GetUserAllChatIdsAsync(userId, cancellationToken);

                if (!result.Succeeded)
                    return Result.Failure("Error retrieving chats for user.");

                var chatIds = result.Data?.Data.ToList();

                if (chatIds == null)
                    return Result.Failure("Error retrieving chats for user.");

                foreach (var chatId in chatIds)
                {
                    _userConnections[userId].Remove((connectionId, chatId));
                }

                return Result.Success();

            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public Result LeaveAllChatsAsync(string connectionId, int userId, List<int> chatIds)
        {
            try
            {
                if (chatIds == null || !chatIds.Any())
                    return Result.Failure("User is not a member of any chat.");

                foreach (var chatId in chatIds)
                {
                    _userConnections[userId].Remove((connectionId, chatId));
                }

                return Result.Success();

            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }  
     
        public async Task<Result> SendMessageAsync(AddMessageDto dto,int userId, int chatId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (!_userConnections[userId].Any(x => x.ChatId == chatId))
                    return Result.Failure("You cannot send messages. You should join a chat first");

                var result = await _messageService.AddMessageAsync(dto,userId,chatId);

               if(result.Succeeded)
               {
                    await _hubContext.Clients.Group(chatName(chatId)).ReceiveChatMessage(result.Data!);

                    return Result.Success();
               }
               else 
                    return Result.Failure("Failed to save the msg to database");
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Database update failed.");
                return Result<int>.Failure($"Database error occurred: {ex.Message}. Please try again.");
            }
            catch (TimeoutException ex)
            {
                //_logger.LogError(ex, "Operation timed out.");
                return Result<int>.Failure($"Request timed out: {ex.Message}. Please try again.");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unexpected error.");
                return Result<int>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

    }
}
