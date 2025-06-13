using MessengerClone.API.General;
using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Service.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(IUserContext _userContext,IUserService _userService) : ControllerBase
    {
        [HttpGet("me",Name ="GetCurrentUserProfile")]
        public async Task<IActionResult> GetUserAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.GetUserByIdAsync(_userContext.UserId, cancellationToken);

                return result.Succeeded 
                     ? SuccessResponse(result.Data, "User retrieved successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int Id, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_User_ID", "User id not valid.", "User Id should be positive number");
         
            try
            {
                var result = await _userService.GetUserByIdAsync(Id, cancellationToken);

                return result.Succeeded
                      ? SuccessResponse(result.Data, "User retrieved successfully.")
                      : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
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


        [HttpGet("{email}", Name = "GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmailAsync([FromRoute] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequestResponse("INVALID_EMAIL", "Email is required.", "Email is required");

            if (!Regex.IsMatch(email, GlobalSettings.EmailPattern))
                return BadRequestResponse("INVALID_EMAIL_FORMAT", "Email format is incorrect.", "Provide a valid email.");

            try
            {
                var result = await _userService.GetUserByEmailAsync(email, cancellationToken);

                return result.Succeeded
                      ? SuccessResponse(result.Data, "User retrieved successfully.")
                      : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
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


        [HttpPut(Name = "UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromForm] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.UpdateUserAsync(_userContext.UserId, dto, cancellationToken);

                return result.Succeeded
                     ? SuccessResponse(result.Data, "User updated successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPost("avatar", Name = "AddUserProfileImag")]
        public async Task<IActionResult> AddUserProfileImag([FromForm] AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.AddUserProfileImageAsync(_userContext.UserId, dto, cancellationToken);
                return result.Succeeded
                     ? SuccessResponse(result.Data, "User image added successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch("avatar/reset",Name = "UpdateUserProfileImag")]
        public async Task<IActionResult> UpdateUserProfileImag([FromForm] AddUpdateUserProfileImageDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.UpdateUserProfileImageAsync(_userContext.UserId,dto,cancellationToken);
                return result.Succeeded
                     ? SuccessResponse(result.Data, "User's image updated successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpDelete("avatar", Name = "DeleteUserProfileImag")]
        public async Task<IActionResult> DeleteUserProfileImag(CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.DeleteUserProfileImageAsync(_userContext.UserId,cancellationToken);
                return result.Succeeded
                     ? SuccessResponse(result.Data, "User updated successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/lock-toggle", Name = "LockOrUnlockUser")]
        public async Task<IActionResult> LockUnLockUserAsync([FromRoute] int Id,[FromBody] LockUnlockDto dto, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_USER_ID", "User id not valid.", "User Id should be positive number");

            try
            {

                var result = dto.Lock
                  ? await _userService.LockUserAsync(Id, cancellationToken)
                  : await _userService.UnLockUserAsync(Id, cancellationToken);

                return result.Succeeded
                     ? SuccessResponse(result.Data, $"User {(result.Data!.locked ? "locked" : "unlocked") } successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpDelete(Name = "DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _userService.DeleteUserAsync(_userContext.UserId, cancellationToken);

                return result.Succeeded
                     ? SuccessResponse("User deleted successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());
            }

            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}", Name = "DeleteUserById")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int Id, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_USER_ID", "User id not valid.", "User Id should be positive number"); 
            
            try
            {
                var result = await _userService.DeleteUserAsync(Id, cancellationToken);

                return result.Succeeded
                     ? SuccessResponse("User deleted successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());
            }

            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error occurred.", $"Unexpected error: {ex.Message}");
            }
        }
    }
}
