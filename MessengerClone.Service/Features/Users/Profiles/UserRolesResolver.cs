using AutoMapper;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Identity;

namespace MessengerClone.Service.Features.Users.Profiles
{
    public class UserRolesResolver : IValueResolver<ApplicationUser, UserDto, List<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRolesResolver(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<string> Resolve(ApplicationUser source, UserDto destination, List<string> destMember, ResolutionContext context)
        {
            try
            {
                var roles = _userManager.GetRolesAsync(source).Result;
                return roles.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
