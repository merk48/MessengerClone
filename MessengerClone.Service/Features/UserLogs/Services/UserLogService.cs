using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.UserLogs.DTOs;
using MessengerClone.Service.Features.UserLogs.Interfaces;

namespace MessengerClone.Service.Features.UserLogs.Services
{
    public class UserLogService(IUnitOfWork _unitOfWork, IMapper _mapper) : IUserLogService
    {
        public async Task<Result> LogAsync(AddLogUserDto dto)
        {
			try
			{
				UserLog userLog = _mapper.Map<UserLog>(dto);

				//,opt =>
    //             {
    //                 opt.Items["UserId"] = userId;
    //             }
                await _unitOfWork.Repository<UserLog>().AddAsync(userLog);

				var saveResult = await _unitOfWork.SaveChangesAsync();

				return saveResult.Succeeded
					? Result.Success()
					: Result.Failure("Failed to log user event");

			}
			catch (Exception)
            {
				return Result.Failure("Failed to log user event");
            }
        }
    }
}
