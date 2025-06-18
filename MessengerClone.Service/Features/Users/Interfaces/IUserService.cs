using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Service.Features.Users.Services;


namespace MessengerClone.Service.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByEmailAsync(string Email, CancellationToken cancellationToken);
        Task<Result<UserDto>> GetUserByIdAsync(int Id, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken); 
        Task<Result<bool>> IsLockedOutAsync(ApplicationUser user, CancellationToken cancellationToken); 
        Task<Result<List<string>>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken);
        Task<Result<UserDto>> UpdateUserAsync(int Id ,UpdateUserDto updateDto, CancellationToken cancellationToken);
        Task<Result<UserDto>> UpdateUserProfileImageAsync(int Id, AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken);
        Task<Result<UserDto>> DeleteUserProfileImageAsync(int Id, CancellationToken cancellationToken);
        Task<Result<UserDto>> AddUserProfileImageAsync(int Id, AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken);
        Task<Result> DeleteUserAsync(int Id, CancellationToken cancellationToken);
        Task<Result<UserDto>> LockUserAsync(int Id, CancellationToken cancellationToken);
        Task<Result<UserDto>> UnLockUserAsync(int Id, CancellationToken cancellationToken);
    }
}
