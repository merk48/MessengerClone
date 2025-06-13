using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.UserLogs.DTOs;

namespace MessengerClone.Service.Features.UserLogs.Interfaces
{
    public interface IUserLogService
    {
        Task<Result> LogAsync(AddLogUserDto dto);
    }
}
