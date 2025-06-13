using MessengerClone.Domain.Abstractions;
using System.Security.Claims;

namespace MessengerClone.API.General
{
    public class HttpUserContext(IHttpContextAccessor _httpCtx) : IUserContext
    {
        public int UserId =>
            int.Parse(_httpCtx.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new InvalidOperationException("UserId claim missing"));

        public string? UserName =>
            _httpCtx.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public IReadOnlyCollection<string> Roles =>
         _httpCtx.HttpContext?.User?
             .FindAll(ClaimTypes.Role)
             .Select(c => c.Value)
             .ToArray() ?? Array.Empty<string>();

    }
}
