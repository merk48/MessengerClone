using FluentValidation;
using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.Auth.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using static MessengerClone.API.Response.ApiResponseHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MessengerClone.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService _authService, IUserContext _userContext) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(dto, cancellationToken);


                return result.Succeeded
                    ? SuccessResponse<TokenDto>(result.Data!, "User logged successfully.")
                    : UnauthorizedResponse("LOGIN_FAILED", "Loging failed", result.ToString());

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


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RefreshToken(dto, cancellationToken);

                return result.Succeeded
                    ? SuccessResponse<TokenDto>(result.Data!, "User logged successfully.")
                    : UnauthorizedResponse("TOKEN_REFRESH_FAILED", "Token refreshing failed", result.ToString());

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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto, [FromServices] IValidator<RegisterDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                                 .Select(e => e.ErrorMessage)
                                 .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in registration.", errors);
            }

            try
            {
                var result = await _authService.RegisterAsync(dto, cancellationToken);

                return result.Succeeded 
                    ? CreatedResponse("GetUserById", new { id = result.Data?.Id }, result.Data, "User registered successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "CREATION_ERROR", result.ToString());
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


        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                await _authService.LogoutAsync(_userContext.UserId, cancellationToken);

                return SuccessResponse("User logged out successfully.");
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
    }
}

// Later log user events
// Later refresh token nd JWE 