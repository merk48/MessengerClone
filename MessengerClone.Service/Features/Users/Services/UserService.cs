using AutoMapper;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Auth.Services;
using MessengerClone.Service.Features.Files.Helpers;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.UserLogs.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Service.Features.Users.Interfaces;
using MessengerClone.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;


namespace MessengerClone.Service.Features.Users.Services
{
    public class UserService(IUnitOfWork _unitOfWork, UserManager<ApplicationUser> _userManager, IMapper _mapper
        , IHttpContextAccessor _httpContextAccessor, IUserLogService _userLogService, IFileService _FileService, ILogger<UserService> _logger)
        : IUserService
    {
      
        public async Task<Result<UserDto>> GetUserByEmailAsync(string Email, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByEmailAsync(Email);

                if (entity == null)
                {
                    _logger.LogWarning("User {UserEmail} not found in {Method}", Email, nameof(GetUserByEmailAsync));
                    return Result<UserDto>.Failure("No user found with the provided email!");
                }


                UserDto userDto = await MapperHelper.BuildUserDto(entity,_userManager,_mapper);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserEmail} in {Method}", Email, nameof(GetUserByEmailAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByEmailAsync for user {UserEmail}", Email);
                return Result<UserDto>.Failure("Failed to retrieve user from the database");
            }
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(GetUserByIdAsync));
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id, nameof(GetUserByIdAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserByIdAsync for user {UserId}", Id);
                return Result<UserDto>.Failure("Failed to retrieve user from the database");
            }
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByEmailAsync(email);

                return entity != null;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserEmail} in {Method}", email, nameof(EmailExistsAsync));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in fetching for user {UserEmail}", email);
                return false;
            }
        }

        public async Task<Result<UserDto>> LockUserAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("Attempted to lock non-existent user {UserId}", Id);
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                var roles = await _userManager.GetRolesAsync(entity);

                if (roles.Contains(AppUserRoles.RoleAdmin))
                {
                    _logger.LogWarning("Attempted to lock an admin account user {UserId}", Id);
                    return Result<UserDto>.Failure("Admin accounts cannot be locked out.");
                }

                if (!entity.LockoutEnabled)
                {
                    _logger.LogWarning("Lockout attempted for user {UserId} with LockoutDisabled", entity.Id);
                    return Result<UserDto>.Failure("User lockout unenabled, you can't lock this user!");
                }

                bool isLocked = await _userManager.IsLockedOutAsync(entity);

                if (isLocked)
                    return Result<UserDto>.Failure("User is already locked!");

                var lockResult = await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
                if (!lockResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to update user to the database");

                var resetAccessFailedCountResult = await _userManager.ResetAccessFailedCountAsync(entity);
                if (!resetAccessFailedCountResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to update user to the database");


                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.AccountLockedout, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} locked out successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
                  
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id, nameof(LockUserAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking out user {UserId}", Id);
                return Result<UserDto>.Failure("Failed to update user to the database");
            }
        }
       
        public async Task<Result<UserDto>> UnLockUserAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("Attempted to unlock non-existent user {UserId}", Id);
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                var roles = await _userManager.GetRolesAsync(entity);

                if (roles.Contains(AppUserRoles.RoleAdmin))
                {
                    _logger.LogWarning("Attempted to unlock an admin account user {UserId}", Id);
                    return Result<UserDto>.Failure("Admin accounts cannot be unlocked out.");
                }

                bool isLocked = await _userManager.IsLockedOutAsync(entity);

                if (!isLocked)
                    return Result<UserDto>.Failure("User is already un locked!");

                var unlockResult = await _userManager.SetLockoutEndDateAsync(entity, null);

                if (!unlockResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to update user to the database");


                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);


                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.AccountUnLocked, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} unlocked out successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
                 
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id, nameof(UnLockUserAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking out user {UserId}", Id);
                return Result<UserDto>.Failure("Failed to update user to the database");
            }
        }

        public async Task<Result<UserDto>> UpdateUserAsync(int Id, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(UpdateUserAsync));
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                _mapper.Map(dto, entity);
                //entity = _mapper.Map<ApplicationUser>(dto);

                var updateResult = await _userManager.UpdateAsync(entity);

                if (!updateResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to update user to the database");


                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.ProfileUpdated, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} updated successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id, nameof(UpdateUserAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId} to the database", Id);
                return Result<UserDto>.Failure("Failed to update user to the database");
            }
        }

        public async Task<Result<UserDto>> AddUserProfileImageAsync(int Id, AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken)
        {
            string imageUrl = "";

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(AddUserProfileImageAsync));
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                var saveAvatarImageResult = await _FileService.SaveAsync(dto.ProfileImage!,enFileCategory.Avatar, entity.Id);

                if (!saveAvatarImageResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to save user's profile image");

                imageUrl = saveAvatarImageResult.Data!;
                entity.ProfileImageUrl = saveAvatarImageResult.Data!;

                var updateResult = await _userManager.UpdateAsync(entity);

                if (!updateResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to add user's profile image to the database");


                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.ProfileUpdated, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} profile image added successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id, nameof(AddUserProfileImageAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user {UserId} profile image to the database", Id);
                return Result<UserDto>.Failure("Failed to add user's profile image to the database");
            }
        }

        public async Task<Result<UserDto>> UpdateUserProfileImageAsync(int Id, AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken)
        {
            string imageUrl = "";

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(UpdateUserProfileImageAsync));
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                var saveAvatarImageResult = await _FileService.ReplaceAsync(dto.ProfileImage!, entity.ProfileImageUrl, enFileCategory.Avatar, entity.Id);

                if (!saveAvatarImageResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to save user's profile image");

                imageUrl = saveAvatarImageResult.Data!;
                entity.ProfileImageUrl = saveAvatarImageResult.Data!;

                var updateResult = await _userManager.UpdateAsync(entity);

                if (!updateResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to update user's profile image to the database");


                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.ProfileUpdated, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} profile image updated successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id,nameof(UpdateUserProfileImageAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId} profile image to the database", Id);
                return Result<UserDto>.Failure("Failed to update user's profile image to the database");
            }
        }
         
        public async Task<Result<UserDto>> DeleteUserProfileImageAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(DeleteUserProfileImageAsync));
                    return Result<UserDto>.Failure("No user found with the provided Id!");
                }

                var deleteAvatarImageResult = await _FileService.DeleteAsync(entity.ProfileImageUrl);

                if (!deleteAvatarImageResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to delete user's profile image");

                entity.ProfileImageUrl = null;

                var updateResult = await _userManager.UpdateAsync(entity);

                if (!updateResult.Succeeded)
                    return Result<UserDto>.Failure("Failed to delete user's profile image from the database");
            

                UserDto userDto = await MapperHelper.BuildUserDto(entity, _userManager, _mapper);

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.ProfileUpdated, _userLogService, _httpContextAccessor);
                _logger.LogInformation("User {UserId} profile image deleted successfully", entity.Id);

                return Result<UserDto>.Success(userDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id,nameof(DeleteUserProfileImageAsync));
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId} profile image from the database", Id);
                return Result<UserDto>.Failure("Failed to delete user's profile image to the database");
            }
        }

        public async Task<Result> DeleteUserAsync(int Id, CancellationToken cancellationToken)
        {

            var hasOwnTr = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<UserDto>.Failure("Falied to start a transaction.");

                ApplicationUser? entity = await _userManager.FindByIdAsync(Id.ToString());

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found in {Method}", Id, nameof(DeleteUserAsync));

                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result.Failure("No user found with the provided Id!");
                }

                var roles = await _userManager.GetRolesAsync(entity);

                if (roles.Contains(AppUserRoles.RoleAdmin))
                {
                    _logger.LogWarning("Attempted to delete an admin account user {UserId}", Id);
                    return Result<UserDto>.Failure("Admin accounts cannot be deleted.");
                }

                string profileimageUr = entity.ProfileImageUrl ?? "";

                await UserLogHelper.LogUserEventAsync(entity.Id, enUserLogEvent.AccountDeleted, _userLogService, _httpContextAccessor);

                var deleteResult = await _userManager.DeleteAsync(entity);

                if(!deleteResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result.Failure("Failed to delete user from the database");
                }

                if (!string.IsNullOrWhiteSpace(profileimageUr))
                {
                    var deleteAvatarImageResult = await _FileService.DeleteAsync(profileimageUr);

                    if (!deleteAvatarImageResult.Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<UserDto>.Failure("Failed to save user's profile image");
                    }
                }


                if (hasOwnTr)
                {
                    var commitTrResult = await _unitOfWork.CommitAsync();
                    if (!commitTrResult.Succeeded)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Result<UserDto>.Failure("Failed to register the user to the database.");
                    }
                }

                _logger.LogInformation("User {UserId} deleted successfully", entity.Id);

                return Result.Success();
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled for user {UserId} in {Method}", Id,nameof(DeleteUserAsync));
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId} from the database", Id);
                return Result.Failure("Failed to delete user from the database");
            }
        }

    }

}
