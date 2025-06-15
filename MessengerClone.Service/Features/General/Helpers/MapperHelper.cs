using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Service.Features.Users.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;

namespace MessengerClone.Service.Features.General.Helpers
{
    public static class MapperHelper
    {
        public static async Task<MessageDto> BuildMessageDto(Message message, IUserService _userService, IMapper _mapper, CancellationToken cancellationToken)
        {
            var isLockedResult = await _userService.IsLockedOutAsync(message.Sender, cancellationToken);
            if (!isLockedResult.Succeeded)
                throw new InvalidOperationException("User lock out status fetch failed.");

            var rolesResult = await _userService.GetRolesAsync(message.Sender, cancellationToken);
            if (!rolesResult.Succeeded)
                throw new InvalidOperationException("User roles fetch failed.");

            return _mapper.Map<MessageDto>(message, opt => {
                opt.Items["IsLocked"] = isLockedResult.Data;
                opt.Items["Roles"] = rolesResult.Data;
            });
        }

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
            var unreadCountResult = await _messageStatusService.GetChatUnreadMessagesCountForUserAsync(chat.Id, currentUserId);
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
