using MessengerClone.Domain.Utils.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MessengerClone.API.Authorization
{
    public class PermissionBasedAuthorizationFilter : IAuthorizationFilter
    {
        // we should use async it is better but this for testing
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var attribute = (CheckPermissionAttribute)context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute)!;
            if (attribute != null)
            {
                var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;

                if (claimIdentity == null || !claimIdentity.IsAuthenticated)
                    context.Result = new ForbidResult(); // 403
                else
                {
                   if (int.TryParse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.ToString(), out int userId))
                    {
                        //var hasPermission = _dbContext.UserPermissions.Any(x => x.UserId == UserSecretsIdAttribute &&
                        //x.PermissionId == attribute.Permission);
                        var hasPermission = ((enPermission)1 == attribute.Permission);

                        if(!hasPermission)
                            context.Result = new ForbidResult(); // 403

                    }
                }
            }
        }
    }
}
