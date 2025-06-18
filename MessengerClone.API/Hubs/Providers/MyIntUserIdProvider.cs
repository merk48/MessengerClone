using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MessengerClone.API.Hubs.Providers
{
    public class MyIntUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            foreach (var claim in connection.User?.Claims ?? Enumerable.Empty<Claim>())
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

}
