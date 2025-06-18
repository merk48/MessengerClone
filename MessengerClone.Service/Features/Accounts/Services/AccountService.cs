using AutoMapper;
using MessengerClone.API.ConfigurationOptions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Constants.Email;
using MessengerClone.Domain.Utils.Constants.SMS;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Account.Interfaces;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.UserLogs.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;
using Twilio.Jwt.AccessToken;

namespace MessengerClone.Service.Features.Account.Services
{
    public class AccountService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager, IMapper _mapper,
                    IEmailSender _emailSender, ISmsSender _smsSender, IOptions<AppSettings> _appSettings
                    , IHttpContextAccessor _httpContextAccessor, IUserLogService _userLogService) 
        : IAccountService
    {

        public async Task<Result> SendVerificationAsync(int userId, enVerificationType type, string? newValue = null)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());


                if (user is null)
                    return Result<UserDto>.Failure("User account not found");

                string token, destination;

                switch (type)
                {
                    case enVerificationType.EmailConfirmation:
                        token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        destination = user.Email!;
                        break;
                    case enVerificationType.PhoneConfirmation:
                        token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, newValue!);
                        destination = newValue;
                        break;
                    case enVerificationType.PasswordReset:
                        token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        destination = user.Email!;
                        break;
                    case enVerificationType.PasswordChange:
                        token = await _userManager.GeneratePasswordResetTokenAsync(user); // if you extend Identity
                        destination = user.Email!;
                        break;
                    case enVerificationType.EmailChange:
                        token = await _userManager.GenerateChangeEmailTokenAsync(user, newValue!);
                        destination = newValue!;
                        break;
                    case enVerificationType.PhoneChange:
                        token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, newValue!);
                        destination = newValue!;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                // 2) Dispatch via SMS or Email
                switch (type)
                {
                    case enVerificationType.PhoneConfirmation:
                    case enVerificationType.PhoneChange:
                        // SMS only needs a plain‐text body
                        await _smsSender.SendSmsAsync(
                            destination!,
                            $"Your {_appSettings.Value.AppName} verification code is: {token}");
                        break;

                    default:
                        // pick subject + HTML body from our EmailTemplates
                        var (subject, html) = BuildEmail(type, token, user.Id, newValue!);
                        await _emailSender.SendEmailAsync(
                            email: destination!,
                            subject: subject,
                            htmlMessage: html);
                        break;
                }

                return Result.Success();
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result.Failure("Failed to send verfication email");
            }
        }

        public async Task<Result<UserDto>> ConfirmVerificationAsync(int userId, enVerificationType type, string token, string? newValue = null)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                    return Result<UserDto>.Failure("User account not found");

                IdentityResult result = new();
                switch (type)
                {
                    case enVerificationType.EmailConfirmation:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.EmailConfirmed, _userLogService, _httpContextAccessor);
                        result = await _userManager.ConfirmEmailAsync(user, token);
                        break;
                    case enVerificationType.PhoneConfirmation:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.PhoneConfirmed, _userLogService, _httpContextAccessor);
                        result = await _userManager.ChangePhoneNumberAsync(user, newValue!, token);
                        break;
                    case enVerificationType.PasswordReset:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.PasswordReset, _userLogService, _httpContextAccessor);
                        result = await _userManager.ResetPasswordAsync(user, token, newValue!);
                        break;
                    case enVerificationType.PasswordChange:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.PasswordChanged, _userLogService, _httpContextAccessor);
                        result = await _userManager.ResetPasswordAsync(user, token, newValue!);
                        break;
                    case enVerificationType.EmailChange:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.EmailChanged, _userLogService, _httpContextAccessor);
                        result = await _userManager.ChangeEmailAsync(user, newValue!, token);
                        break;
                    case enVerificationType.PhoneChange:
                        await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.PhoneChanged, _userLogService, _httpContextAccessor);
                        result = await _userManager.ChangePhoneNumberAsync(user, newValue!, token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                if(!result.Succeeded)
                    return Result<UserDto>.Failure("Failed to Confirm verfication");

                var userDto = _mapper.Map<UserDto>(user);

                return userDto is not null
                    ? Result<UserDto>.Success(userDto)
                    : Result<UserDto>.Failure("Failed to Confirm verfication");

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<UserDto>.Failure("Failed to Confirm verfication email");
            }
        }

        public async Task<Result> DeactivateAccountAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                    return Result.Failure("User account not found");

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(enAppUserRoles.Admin.ToString()))
                    return Result.Failure("Admin accounts cannot be deactivated.");

                if (!user.IsActive)
                    return Result.Failure("Account is already deactive.");

                user.IsActive = false;
                user.DeactivatedAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return Result.Failure("Failed to deactivate user's account.");

                await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.AccountDeactivated, _userLogService, _httpContextAccessor);


                return Result.Success();

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result.Failure("Failed to deactivate user's account.");
            }
        }

        public async Task<Result> ReactivateAccountAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user is null)
                    return Result.Failure("User account not found");

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(enAppUserRoles.Admin.ToString()))
                    return Result.Failure("Admin accounts cannot be reactivated.");

                if (user.IsActive)
                    return Result.Failure("Account is already active.");

                user.IsActive = true;
                user.DeactivatedAt = null;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return Result.Failure("Failed to reactivate user's account.");

                await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.AccountReactivated, _userLogService, _httpContextAccessor);

                return Result.Success();

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result.Failure("Failed to reactivate user's account.");
            }
        }


        /// <summary>
        /// Returns the email subject and HTML body for each VerificationType.
        /// </summary>
        private (string subject, string html) BuildEmail(enVerificationType type, string token, int userId, string newValue)
        {
            switch (type)
            {
                case enVerificationType.EmailConfirmation:
                    return (
                        subject: $"Confirm your {_appSettings.Value.AppName} email",
                        html: EmailTemplates.VerificationCode(token, "email")
                    );

                case enVerificationType.PasswordReset:
                    // generate a password‐reset link
                    var resetLink = GenerateLink(
                        "reset-password",
                        ("userId", userId.ToString()),
                        ("token", HttpUtility.UrlEncode(token))
                    );
                    return (
                        subject: $"Reset your {_appSettings.Value.AppName} password",
                        html: EmailTemplates.PasswordReset(resetLink)
                    );

                case enVerificationType.PasswordChange:
                    // same as reset, but different subject
                    var changeLink = GenerateLink(
                        "change-password",
                        ("userId", userId.ToString()),
                        ("token", HttpUtility.UrlEncode(token))
                    );
                    return (
                        subject: $"Change your {_appSettings.Value.AppName} password",
                        html: EmailTemplates.PasswordReset(changeLink)
                    );

                case enVerificationType.EmailChange:
                    // code‐flow email change
                    return (
                        subject: $"Confirm your new {_appSettings.Value.AppName} email",
                        html: EmailTemplates.ChangeEmailCode(token, newValue)
                    );

                case enVerificationType.PhoneConfirmation:
                case enVerificationType.PhoneChange:
                    // these are handled in SMS section—won’t hit here
                    throw new InvalidOperationException("Should be sent via SMS");

                default:
                    // fallback to generic code
                    return (
                        subject: $"Your {_appSettings.Value.AppName} verification code",
                        html: EmailTemplates.VerificationCode(token, "verification")
                    );
            }
        }

        /// <summary>
        /// Combines your app’s BaseUrl with a route and query parameters.
        /// </summary>
        private string GenerateLink(string route, params (string key, string value)[] qs)
        {
            // route to html page

            var baseUrl = _appSettings.Value.BaseUrl
                      ?? throw new InvalidOperationException("Missing App:BaseUrl");
            var builder = new StringBuilder($"{baseUrl.TrimEnd('/')}/{route}?");
            foreach (var (k, v) in qs)
            {
                builder.Append($"{k}={v}&");
            }
            // remove trailing &
            builder.Length--;
            return builder.ToString();
        }

        //public async Task<Result<UserDto>> ChangeEmailAsync(int currentUserId, ChangeEmailDto dto)
        //{
        //    try
        //    {
        //        var entity = await _userManager.FindByIdAsync(currentUserId.ToString());

        //        if (entity is null)
        //            return Result<UserDto>.Failure("User account not found");

        //        if (entity.Email != dto.OldEmail)
        //            return Result<UserDto>.Failure("Old email is incorrect");

        //        // confirm the old email first 
        //        // will send email to go to another page for accessing this api

        //        // change it
        //        entity.Email = dto.NewEmail;

        //        // ask for this 
        //        await _userManager.UpdateNormalizedEmailAsync(entity);

        //        var userDto = _mapper.Map<UserDto>(entity);

        //        return userDto is not null
        //            ? Result<UserDto>.Success(userDto)
        //            : Result<UserDto>.Failure("Failed to change user's acoount email to the database");

        //    }
        //    catch (Exception)
        //    {
        //        //Log.Error(ex.Message);
        //        return Result<UserDto>.Failure("Failed to change user's acoount email to the database");
        //    }
        //}

        //public async Task<Result<UserDto>> ChangePasswordAsync(int currentUserId, ChangePasswordDto dto)
        //{
        //    try
        //    {
        //        var entity = await _userManager.FindByIdAsync(currentUserId.ToString());

        //        if (entity is null)
        //            return Result<UserDto>.Failure("User account not found");

        //        var passwordCorrect = await _userManager.CheckPasswordAsync(entity, dto.OldPassword);

        //        if (!passwordCorrect)
        //            return Result<UserDto>.Failure("Old password is incorrect");

        //        // confirm the old email first 
        //        // then will redirect to this page for entering the new password then will call this api

        //        entity.PasswordHash = dto.NewPassword;

        //        await _userManager.UpdateAsync(entity);

        //        var userDto = _mapper.Map<UserDto>(entity);

        //        return userDto is not null
        //            ? Result<UserDto>.Success(userDto)
        //            : Result<UserDto>.Failure("Failed to change user's account password to the database");

        //    }
        //    catch (Exception)
        //    {
        //        //Log.Error(ex.Message);
        //        return Result<UserDto>.Failure("Failed to change user's account password to the database");
        //    }
        //}

        //public async Task<Result<UserDto>> ResetPasswordAsync(ResetForgetPasswordDto dto)
        //{
        //    try
        //    {
        //        var entity = await _userManager.FindByEmailAsync(dto.Email);

        //        if (entity is null)
        //            return Result<UserDto>.Failure("User account not found");

        //        // confirm the email first 
        //        // send to it a link to the page that will handle entwring the new passwrod and from then should call this api

        //        entity.PasswordHash = dto.NewPassword;

        //        await _userManager.UpdateAsync(entity);

        //        var userDto = _mapper.Map<UserDto>(entity);

        //        return userDto is not null
        //            ? Result<UserDto>.Success(userDto)
        //            : Result<UserDto>.Failure("Failed to reset user's account password to the database");

        //    }
        //    catch (Exception)
        //    {
        //        //Log.Error(ex.Message);
        //        return Result<UserDto>.Failure("Failed to reset user's account password to the database");
        //    }
        //}

        //public async Task<Result<UserDto>> ChangePhoneNumberAsync(int currentUserId, ChangePhoneNumberDto dto)
        //{
        //    try
        //    {
        //        var entity = await _userManager.FindByIdAsync(currentUserId.ToString());

        //        if (entity is null)
        //            return Result<UserDto>.Failure("User account not found");

        //        // confirm the phone number first with code to the phonenumber like example
        //        // send to it a link to the page that will handle entwring the new phone number then will call this api

        //        entity.PhoneNumber = dto.NewPhoneNumber;

        //        await _userManager.UpdateAsync(entity);

        //        var userDto = _mapper.Map<UserDto>(entity);

        //        return userDto is not null
        //            ? Result<UserDto>.Success(userDto)
        //            : Result<UserDto>.Failure("Failed to reset user's account password to the database");

        //    }
        //    catch (Exception)
        //    {
        //        //Log.Error(ex.Message);
        //        return Result<UserDto>.Failure("Failed to change user's acoount phone number to the database");
        //    }
        //}

    }
}
