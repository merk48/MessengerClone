using AutoMapper;
using MessengerClone.API.ConfigurationOptions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.Auth.Interfaces;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Files.Helpers;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using MessengerClone.Service.Features.UserLogs.DTOs;
using MessengerClone.Service.Features.UserLogs.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MessengerClone.Service.Features.Auth.Services
{
    public class AuthService(IConfiguration configuration, IUnitOfWork _unitOfWork, UserManager<ApplicationUser> _userManager,
                    SignInManager<ApplicationUser> _signInManager, IMapper _mapper, IHttpContextAccessor _httpContextAccessor,
                    IUserLogService _userLogService, IFileService _FileService, ILogger<AuthService> _logger) 
        : IAuthService
    {
      
        public async Task<Result<TokenDto>> LoginAsync(LoginDto dto , CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: No user found with email {Email}", dto.Email);
                    return Result<TokenDto>.Failure("No user found with the provided email!");
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Login attempt for locked out user {UserId} ({Email})", user.Id, user.Email);
                    return Result<TokenDto>.Failure("User account is locked out!");
                }

                if (!user.EmailConfirmed)
                    return Result<TokenDto>.Failure("Email not confirmed!, Please confirm your email before logging in");

                var (ip, ua) = UserLogHelper.GetRequestDetails(_httpContextAccessor);

                //Prevent Brute Force Attacks
                var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, lockoutOnFailure: true);

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account locked due to multiple failed login attempts for user {UserId} ({Email})", user.Id, user.Email);
                    return Result<TokenDto>.Failure("Account locked due to multiple failed attempts!");
                }

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed: Invalid password for user {UserId} ({Email}) from IP {IP} with UserAgent {UserAgent}"
                        , user.Id, user.Email, ip, ua);

                    await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.LoginFailed, _userLogService, _httpContextAccessor, ip, ua);

                    return Result<TokenDto>.Failure("Invalid  credentials!");
                }


                _logger.LogInformation("User {UserId} ({Email}) logged in successfully from IP {IP} with UserAgent {UserAgent}",
                       user.Id, user.Email, ip, ua);

                // Generate loging token
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                var newToken = GenerateToken(user, roles);

                // Clean up access failed count
                var resetAFCountResult = await _userManager.ResetAccessFailedCountAsync(user);
                if (!resetAFCountResult.Succeeded)
                    return Result<TokenDto>.Failure("Loging failed!");

                // Update refresh token
                var refreshToken = GenerateRefreshToken();

                if (!(await UpdateUserRefreshToken(user, refreshToken)).Succeeded)
                    return Result<TokenDto>.Failure("Loging failed!");


                await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.TokenRefreshed, _userLogService, _httpContextAccessor, ip, ua);
                await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.LoginSuccess, _userLogService, _httpContextAccessor, ip, ua);


                return Result<TokenDto>.Success(new TokenDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(newToken),
                    RefreshToken = refreshToken,
                    ExpiredTime = newToken.ValidTo
                });

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Registration request was canceled for user {Email}", dto.Email);
                return Result<TokenDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user {Email}", dto.Email);
                return Result<TokenDto>.Failure("Login failed due to server error.");
            }
        }

        public async Task<Result<TokenDto>> RefreshToken(RefreshTokenRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken &&
                                         u.RefreshTokenExpiryTime > DateTime.UtcNow);

                if (user == null)
                {
                    _logger.LogWarning("Refresh token failed: invalid or expired token attempted");
                    return Result<TokenDto>.Failure("Invalid refresh token!");
                }

                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                var newToken = GenerateToken(user, roles);
                var newRefreshToken = GenerateRefreshToken();

                if (!(await UpdateUserRefreshToken(user,newRefreshToken)).Succeeded)
                    return Result<TokenDto>.Failure("Loging failed!");

                _logger.LogInformation("Refresh token used for user {UserId} ({Email})", user.Id, user.Email);

                await UserLogHelper.LogUserEventAsync(user.Id, enUserLogEvent.TokenRefreshed, _userLogService, _httpContextAccessor);

                return Result<TokenDto>.Success(new TokenDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(newToken),
                    RefreshToken = newRefreshToken,
                    ExpiredTime = newToken.ValidTo
                });
            }
            catch (OperationCanceledException)
            {
                return Result<TokenDto>.Failure("Request was canceled");
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "Login failed for user {Email}", dto.Email); // Inject ILogger<AuthService>
                return Result<TokenDto>.Failure("Login failed due to server error.");
            }
        }
      
        public async Task<Result<UserDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            var hasOwnTr = false;
            string imageUrl = "";

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Transaction handle
                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<UserDto>.Failure("Falied to start a transaction.");


                ApplicationUser user = _mapper.Map<ApplicationUser>(dto);

               
                var creationResult = await _userManager.CreateAsync(user, dto.Password);
                if (!creationResult.Succeeded)
                {
                    _logger.LogWarning("User registration failed during creation step for email {Email}. Errors: {Errors}",
                            dto.Email, string.Join(", ", creationResult.Errors.Select(e => e.Description)));

                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<UserDto>.Failure("Failed to save the user to the database.");
                }


                #region Handle Profile image

                if (dto.ProfileImage is not null)
                {
                    var saveAvatarImageResult = await _FileService.SaveAsync(dto.ProfileImage, enFileCategory.Avatar, user.Id);

                    if (!saveAvatarImageResult.Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<UserDto>.Failure("Failed to save user's profile image");
                    }

                    imageUrl = saveAvatarImageResult.Data!;
                }

                user.ProfileImageUrl = imageUrl;

                var updateUserResult = await _unitOfWork.SaveChangesAsync();
                if (!updateUserResult.Succeeded)
                {
                    var deleteImageResult = await _FileService.DeleteAsync(user.ProfileImageUrl);
                    if (!deleteImageResult.Succeeded)
                        return Result<UserDto>.Failure(deleteImageResult.ToString());

                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<UserDto>.Failure("Failed to register the user to the database.");
                }

                #endregion


                foreach (var role in dto.Roles)
                {
                    var addRolesResult = await _userManager.AddToRoleAsync(user, role);

                    if (!addRolesResult.Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<UserDto>.Failure("Failed to add role to the user");
                    }
                }


                UserDto userDto = await MapperHelper.BuildUserDto(user, _userManager, _mapper);

                if (!hasOwnTr)
                    return Result<UserDto>.Success(userDto);

                var commitTrResult = await _unitOfWork.CommitAsync();
                if (!commitTrResult.Succeeded)
                {
                    var deleteImageResult = await _FileService.DeleteAsync(user.ProfileImageUrl);
                    if (!deleteImageResult.Succeeded)
                        return Result<UserDto>.Failure(deleteImageResult.ToString());

                    await _unitOfWork.RollbackAsync();
                    return Result<UserDto>.Failure("Failed to register the user to the database.");
                }

                _logger.LogInformation("User registered successfully with email {Email} and roles {Roles}", dto.Email, string.Join(", ", dto.Roles));

                return Result<UserDto>.Success(userDto);

            }
            catch (OperationCanceledException)
            {

                if(!string.IsNullOrEmpty(imageUrl))
                {
                    var deleteImageResult = await _FileService.DeleteAsync(imageUrl);
                    if (!deleteImageResult.Succeeded)
                        return Result<UserDto>.Failure(deleteImageResult.ToString());
                }

                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<UserDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var deleteImageResult = await _FileService.DeleteAsync(imageUrl);
                    if (!deleteImageResult.Succeeded)
                        return Result<UserDto>.Failure(deleteImageResult.ToString());
                }

                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<UserDto>.Failure("Failed to register the user to the database.");
            }
        }

        public async Task LogoutAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation("User {UserId} logged out", userId);
                await UserLogHelper.LogUserEventAsync(userId, enUserLogEvent.Logout, _userLogService, _httpContextAccessor);

                await _signInManager.SignOutAsync();
            }
            catch (OperationCanceledException)
            {
                //Log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
            }
        }

        private async Task<IdentityResult> UpdateUserRefreshToken(ApplicationUser user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            return (await _userManager.UpdateAsync(user));
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        private static IEnumerable<Claim> GenerateUserClaims(string userName, int userId, IEnumerable<string> roles)
        {
            yield return new Claim(ClaimTypes.Name, userName);
            yield return new Claim(ClaimTypes.NameIdentifier, userId.ToString());
            yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

            foreach (var role in roles)
                yield return new Claim(ClaimTypes.Role, role);
        }

        private JwtSecurityToken GenerateToken(ApplicationUser user, List<string> roles)
        {
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: configuration["JWT:Issuer"],
               audience: configuration["JWT:Audience"],
               expires: DateTime.UtcNow.AddMinutes(Convert.ToUInt32(configuration["JWT:Lifetime"])),
               signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])), SecurityAlgorithms.HmacSha256),
               claims: GenerateUserClaims(user.UserName!, user.Id, roles)
               );

            return token;
        }

    }
}
