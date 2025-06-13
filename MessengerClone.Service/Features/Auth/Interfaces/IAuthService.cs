using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
        Task<Result<TokenDto>> RefreshToken(RefreshTokenRequestDto dto, CancellationToken cancellationToken);
        Task LogoutAsync(int userId, CancellationToken cancellationToken);
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
    }
}
