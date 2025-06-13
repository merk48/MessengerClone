using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.UserLogs.DTOs;
using MessengerClone.Service.Features.UserLogs.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.General.Extentions
{
    public static class UserLogHelper
    {
        public static (string Ip, string UserAgent) GetRequestDetails(IHttpContextAccessor accessor)
        {
            var context = accessor.HttpContext;
            var ip = context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            var ua = context?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";
            return (ip, ua);
        }

        public static async Task LogUserEventAsync(int userId, enUserLogEvent logEvent,IUserLogService _userLogService, IHttpContextAccessor _httpContextAccessor, string? ip = null, string? ua = null)
        {
            if(ip is null && ua is null)
                (ip, ua) = UserLogHelper.GetRequestDetails(_httpContextAccessor);

            var logResult = await _userLogService.LogAsync(new AddLogUserDto
            {
                UserId = userId,
                Event = logEvent,
                IpAddress = ip!,
                UserAgent = ua!
            });

            if (!logResult.Succeeded)
                throw new Exception($"Failed to log event {logEvent} for user {userId}");
        }

    }
}
