using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using MessengerClone.Repository.EntityFrameworkCore.Context;
using MessemgerClone.Domain.Entities.Identity;
using MessengerClone.Utilities.Constants;
using MessemgerClone.Repository.DbInitializer;
using MessengerClone.Domain.Entities.Identity;


namespace MessengerClone.Repository.DbInitializer
{
    public class DbInitializer(AppDbContext _context, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager) : IDbInitializer
    {
        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(AppUserRoles.RoleAdmin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.RoleAdmin }).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.RoleMember }).GetAwaiter().GetResult();
                }

                if (!_context.ApplicationUsers.Any())
                {
                    ApplicationUser user = new();
                    user.FirstName = "admin";
                    user.LastName = "admin";
                    user.PhoneNumber = "+9630991037153";
                    user.Email = "admin@gmail.com";
                    user.UserName = "admin@gmail.com";
                    user.LockoutEnabled = false;

                    var result = _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(user, AppUserRoles.RoleAdmin).GetAwaiter().GetResult();

                        // Email Confirmed
                        var codeToConfirm = _userManager.GenerateEmailConfirmationTokenAsync(user).GetAwaiter().GetResult();
                        codeToConfirm = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codeToConfirm));

                        codeToConfirm = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codeToConfirm));
                        _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                        _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                        // Set Lockout Enabled to false
                        _userManager.SetLockoutEnabledAsync(user, false);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                throw new Exception($"Something got wrong while initializing the database: {ex.Message}");
            }
        }
    }
}

