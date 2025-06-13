using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Account.Interfaces
{
    public interface IAccountService
    {
        Task<Result> SendVerificationAsync(int userId, enVerificationType type, string? newValue = null);
        Task<Result<UserDto>> ConfirmVerificationAsync(int userId, enVerificationType type, string token, string? newValue = null);
        Task<Result> DeactivateAccountAsync(int userId);
        Task<Result> ReactivateAccountAsync(int userId);
        //Task<Result<UserDto>> ChangeEmailAsync(int currentUserId, ChangeEmailDto dto);
        //Task<Result<UserDto>> ChangePhoneNumberAsync(int currentUserId, ChangePhoneNumberDto dto);
        //Task<Result<UserDto>> ChangePasswordAsync(int currentUserId, ChangePasswordDto dto);
        //Task<Result<UserDto>> ResetPasswordAsync(ResetForgetPasswordDto dto);
    }
}
