using MessemgerClone.Domain.Entities.Identity;
using MessengerClone.API.ConfigurationOptions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Repository.EntityFrameworkCore.Context;
using MessengerClone.Utilities.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MessengerClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IOptions<EmailSettingsOptions> emailSettingsOptions;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public WeatherForecastController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,IConfiguration configuration,ILogger<WeatherForecastController> logger, IOptions<EmailSettingsOptions> emailSettingsOptions)
    {
        _configuration = configuration;
        _logger = logger;
        this.emailSettingsOptions = emailSettingsOptions;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    //[AllowAnonymous]
    //[HttpGet(Name = "GetWeatherForecast")]
    //public IEnumerable<WeatherForecast> Get()
    //{
    //    var userName = User.Identity.Name;
    //    var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;


    //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //    {
    //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //        TemperatureC = Random.Shared.Next(-20, 55),
    //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //    })
    //    .ToArray();
    //}

    //[AllowAnonymous]
    //[HttpGet("/config")]
    //public IActionResult Config()
    //{
    //    //ApplicationUser user = new();
    //    //user.FirstName = "mery";
    //    //user.LastName = "kassis";
    //    //user.Email = "mery@gmail.com";
    //    //user.UserName = "mery@gmail.com";
    //    //user.LockoutEnabled = false;


    //    //var result = _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();
                       
    //    //if(result.Succeeded)
    //    //    _userManager.AddToRoleAsync(user, AppUserRoles.RoleAdmin).GetAwaiter().GetResult();

    //    var config = new
    //    {
    //        //result = result.ToString(),
    //        AllowHosts = _configuration["AllowedHosts"],
    //        ConnectionString = _configuration.GetConnectionString("DefaultConnection"),
    //        EnvName = _configuration["ASPNETCORE_ENVIRONMENT"],
    //        Key = _configuration["JWT:Secret"],
    //        emailSettingsOptions = emailSettingsOptions.Value
    //    };


    //    return Ok(config);
    //}
}
