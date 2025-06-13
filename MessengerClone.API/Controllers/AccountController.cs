using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Account.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController(IAccountService _accountService, IUserContext _userContext) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("verification", Name = "SendVerficationEmail")]
        public async Task<IActionResult> SendVerficationEmail([FromBody] VerificationRequestDto dto)
        {
            try
            {
                // change this as the logic for all and for auth users
                var result = await _accountService.SendVerificationAsync(1, dto.Type, dto.NewValue);

                return result.Succeeded
                     ? SuccessResponse("Email has sent to the email successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "EMAIL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpPost("verification/confirm", Name = "ConfirmVerficationEmail")]
        public async Task<IActionResult> ConfirmVerficationEmail([FromBody] ConfirmRequestDto dto)
        {
            try
            {
                // change this also
                var result = await _accountService.ConfirmVerificationAsync(1, dto.Type, dto.Token, dto.NewValue);

                return result.Succeeded
                  ? SuccessResponse("Email has confirmed successfully.")
                  : StatusCodeResponse(StatusCodes.Status500InternalServerError, "EMAIL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPost("active-toggle", Name = "DeactivateOrReactivateAccount")]
        public async Task<IActionResult> DeactivateReactivateAccount([FromBody] DeactivateReactivateAccountDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                Result result = new();

                if(dto.IsActive)
                    result = await _accountService.ReactivateAccountAsync(_userContext.UserId);
                else 
                    result = await _accountService.DeactivateAccountAsync(_userContext.UserId);

                return result.Succeeded
                  ? SuccessResponse($"User's account has {(dto.IsActive ? "reactivated": "deactivated")} successfully.")
                  : StatusCodeResponse(StatusCodes.Status500InternalServerError, "EMAIL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/active-toggle", Name = "DeactivateOrReactivateAccountById")]
        public async Task<IActionResult> DeactivateReactivateAccountById([FromRoute] int Id, [FromBody] DeactivateReactivateAccountDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (Id <= 0)
                    return BadRequestResponse("INVALID_User_ID", "User id not valid.", "User Id should be positive number");

                Result result = new();

                if (dto.IsActive)
                    result = await _accountService.ReactivateAccountAsync(_userContext.UserId);
                else
                    result = await _accountService.DeactivateAccountAsync(_userContext.UserId);

                return result.Succeeded
                  ? SuccessResponse($"User's account has {(dto.IsActive ? "reactivated" : "deactivated")} successfully.")
                  : StatusCodeResponse(StatusCodes.Status500InternalServerError, "EMAIL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        //[HttpPatch("password/change")]
        //public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();
        //        return BadRequestResponse("DATA_INVALID", "Validation error in ChangePasswordDto.", errors);
        //    }

        //    try
        //    {
        //        if (_userContext.UserId <= 0)
        //            return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

        //        var result = await _accountService.ChangePasswordAsync(_userContext.UserId, changePasswordDto);

        //        return result.Succeeded
        //            ? SuccessResponse("User's account password reset successfully.")
        //            : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
        //    }
        //}


        //[AllowAnonymous]
        //[HttpPatch("password/reset")]
        //public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetForgetPasswordDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();
        //        return BadRequestResponse("DATA_INVALID", "Validation error in ChangePasswordDto.", errors);
        //    }

        //    try
        //    {
        //        if (_userContext.UserId <= 0)
        //            return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

        //        var result = await _accountService.ResetPasswordAsync(dto);

        //        return result.Succeeded
        //            ? SuccessResponse("User's account email changed successfully.")
        //            : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
        //    }
        //}


        //[HttpPatch("email/change")]
        //public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();
        //        return BadRequestResponse("DATA_INVALID", "Validation error in ChangePasswordDto.", errors);
        //    }

        //    try
        //    {
        //        if (_userContext.UserId <= 0)
        //            return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

        //        var result = await _accountService.ChangeEmailAsync(_userContext.UserId, dto);

        //        return result.Succeeded
        //            ? SuccessResponse("User's account email changed successfully.")
        //            : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
        //    }
        //}


        //[HttpPatch("phonenumber/change")]
        //public async Task<IActionResult> ChangePhoneNumber([FromBody] ChangePhoneNumberDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();
        //        return BadRequestResponse("DATA_INVALID", "Validation error in ChangePasswordDto.", errors);
        //    }

        //    try
        //    {
        //        if (_userContext.UserId <= 0)
        //            return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

        //        var result = await _accountService.ChangePhoneNumberAsync(_userContext.UserId, dto);

        //        return result.Succeeded
        //            ? SuccessResponse("User's account phone number changed successfully.")
        //            : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
        //    }
        //}

    }
}
