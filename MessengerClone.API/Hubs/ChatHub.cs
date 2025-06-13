using MessengerClone.Service.Features.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.API.Hubs.Chathub.Interfaces;
using MessengerClone.Domain.Utils.Global;

namespace MessengerClone.API.Hubs
{
    [Authorize]
    public class ChatHub(IChatHubService _chatHubService) : Hub<IChatClient>
    {
        // lestening to real-time requests

        // Connected and disconnected
        public override async Task OnConnectedAsync()
        {
            if (!int.TryParse(Context.UserIdentifier, out var userId))
                throw new HubException("Invalid user identifier");

            //var result = await _chatHubService.JoinAllChatsAsync(Context.ConnectionId, userId);

            //if (!result.Succeeded)
            //    await Clients.Caller.ReceiveError(result.ToString());

            await base.OnConnectedAsync();
        }
       
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (!int.TryParse(Context.UserIdentifier, out var userId))
                throw new HubException("Invalid user identifier");

            //, CancellationToken cancellationToken
            //var result = await _chatHubService.LeaveAllChatsAsync(Context.ConnectionId, userId, cancellationToken);

            //if (!result.Succeeded)
            //    await Clients.Caller.ReceiveError(result.ToString());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(int chatId,int userId, CancellationToken cancellationToken)
        {
            var result = await _chatHubService.JoinChatAsync(Context.ConnectionId, userId, chatId, cancellationToken);
                
                if (!result.Succeeded)
                    await Clients.Caller.ReceiveError(result.ToString());
        }

        public async Task JoinAllUserChats(List<int> chatsIds)
        {
            if (!int.TryParse(Context.UserIdentifier, out var userId))
                throw new HubException("Invalid user identifier");

            var result = await _chatHubService.JoinAllChatsAsync(Context.ConnectionId, userId, chatsIds);

            if (!result.Succeeded)
                await Clients.Caller.ReceiveError(result.ToString());
        }

        public async Task LeaveChat(int chatId, int userId, CancellationToken cancellationToken)
        {
           
            var result = await _chatHubService.LeaveChatAsync(Context.ConnectionId, userId, chatId, cancellationToken);
                if (!result.Succeeded)
                    await Clients.Caller.ReceiveError(result.ToString());
        }

        public async Task SendMessage(AddMessageDto dto, int chatId, CancellationToken cancellationToken)
        {
                if (!int.TryParse(Context.UserIdentifier, out var userId))
                    throw new HubException("Invalid user identifier");

                var result = await _chatHubService.SendMessageAsync(dto, userId, chatId, cancellationToken);
                if (!result.Succeeded)
                    await Clients.Caller.ReceiveError(result.ToString());
        }
    }

    public interface IChatClient
    {
        // Events
        Task ReceiveChatMessage(MessageDto messageDto);
        Task ReceiveLineMessage(UserDto userDto);
        Task ReceiveError(string errorMsg);
    }
}

// Client -----------------proxy---------------Server
// Define connection --- create proxy ----Start connection
// invoke hub methods --- subscribe to callbacks
// Proxy:
//      Generated (Default) js proxy
//      Custom proxy


// After login:
// 1- connect
// 2- join all conversation => GET /api/conversations/my => to get all the conversation IDs the user belongs to.
//    Loop through them and for each conversation, call connection.invoke("JoinConversation", convId).
// 3- chat load
// 4- able to send and receive when sent


// After login	Start SignalR connectionProzy
//              Call API to get my conversation IDs
//              Join each group via JoinConversation(id)
// When message is sent	Send to group               → everyone in that chat receives it
// When message is received	If it’s active chat     → show in chat; else → ping / update UI
// Logout	                                        Stop connection


//1] I want to apply this: History: load summary(last message) on sidebar, full history on chat open.
//Most apps fetch just the last N messages for the sidebar, then on click load “full” history (or progressive scroll). Then the sidebar itself shows the conversation title + last message, unread badge, etc.

//tell me how to do it

//2]


// Account controller
// Finish auth
// Test Auth
// Test UserID
// Test Chat
// Conversation Get
// Conversation Service right
// Message Service right !!!