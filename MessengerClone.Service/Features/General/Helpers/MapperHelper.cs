using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Identity;
using System;

namespace MessengerClone.Service.Features.General.Helpers
{
    public static class MapperHelper
    {
        public static async Task<UserDto> BuildUserDto(ApplicationUser user, UserManager<ApplicationUser> _userService, IMapper _mapper)
        {
            var isLocked = await _userService.IsLockedOutAsync(user);
            var roles = await _userService.GetRolesAsync(user);
            return _mapper.Map<UserDto>(user, opt => {
                opt.Items["IsLocked"] = isLocked;
                opt.Items["Roles"] = roles;
            });
        }

        public static async Task<ChatMetadataDto> BuildChatMetadataDto(Chat chat, int currentUserId, IMessageStatusService _messageStatusService, IMapper _mapper)
        {
            var unreadCountResult = await _messageStatusService.GetChatUnreadMessagesForUserCountAsync(chat.Id, currentUserId);
            if (!unreadCountResult.Succeeded)
                throw new InvalidOperationException("Unread message count fetch failed.");

            return _mapper.Map<ChatMetadataDto>(chat, opt =>
            {
                opt.Items["CurrentUserId"] = currentUserId;
                opt.Items["UnreadCount"] = unreadCountResult.Data;
            });

        }
    }
}
