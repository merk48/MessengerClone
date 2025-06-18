
using MessemgerClone.Domain.Entities.Identity;
using MessemgerClone.Repository.DbInitializer;
using MessengerClone.API.ConfigurationOptions;
using MessengerClone.API.General;
using MessengerClone.API.Hubs;
using MessengerClone.API.Hubs.Chathub.Interfaces;
using MessengerClone.API.Hubs.Implementations;
using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities.Identity;
using MessengerClone.Domain.IRepository;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Constants.Email;
using MessengerClone.Domain.Utils.Constants.SMS;
using MessengerClone.Repository.DbInitializer;
using MessengerClone.Repository.EntityFrameworkCore.Context;
using MessengerClone.Repository.EntityFrameworkCore.Interceptors;
using MessengerClone.Repository.Repository;
using MessengerClone.Repository.UnitOfWork;
using MessengerClone.Service.Features.Account.Interfaces;
using MessengerClone.Service.Features.Account.Mappers;
using MessengerClone.Service.Features.Account.Services;
using MessengerClone.Service.Features.Auth.Interfaces;
using MessengerClone.Service.Features.Auth.Services;
using MessengerClone.Service.Features.ChatMembers.Services;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.Chats.Services;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.Files.Services;
using MessengerClone.Service.Features.MediaAttachments.Interfaces;
using MessengerClone.Service.Features.MediaAttachments.Services;
using MessengerClone.Service.Features.MessageReactions.Interfaces;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.Services;
using MessengerClone.Service.Features.UserLogs.Interfaces;
using MessengerClone.Service.Features.UserLogs.Services;
using MessengerClone.Service.Features.Users.Interfaces;
using MessengerClone.Service.Features.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation.AspNetCore;
using System.Text;
using MessengerClone.Service.Features.Auth.DTOs;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using MessengerClone.Service.Features.Auth.Validators;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using MessengerClone.Service.Features.Chats.Validators;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.MessageReactions.Services;
using MessengerClone.API.Hubs.Providers;

namespace MessengerClone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

              Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .CreateLogger();

            builder.Host.UseSerilog();

            // EF Core Configuration
            builder.Services.AddScoped<AuditByInterceptor>();
            builder.Services.AddScoped<SoftDeleteInterceptor>();
            builder.Services.AddScoped<AuditAtInterceptor>();

            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var auditByInterceptor = serviceProvider.GetRequiredService<AuditByInterceptor>();
                //var latestChatsUpdateInterceptor = serviceProvider.GetRequiredService<LatestChatsUpdateInterceptor>();
                var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(softDeleteInterceptor,
                                 new AuditAtInterceptor(),
                                 auditByInterceptor);

                // Logging SQL queries to console
                options.LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging() // Optional: shows parameters
                       .EnableDetailedErrors();      // Optional: detailed error messages
            });

            // Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            #region Custom Services
            // Configuration Options 
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            // Custom services 
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IAccountService, AccountService> ();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatMemeberService, ChatMemeberService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IMessageStatusService, MessageStatusService>();
            builder.Services.AddScoped<IMediaAttachmentService, MediaAttachmentService>();
            builder.Services.AddScoped<IMessageReactionService, MessageReactionService>();
            builder.Services.AddScoped<IUserLogService, UserLogService>();
            builder.Services.AddScoped<IFileService, FileService>();


            builder.Services.AddScoped<IChatHubService, ChatHubService>();
            builder.Services.AddSingleton<IUserIdProvider, MyIntUserIdProvider>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContext, HttpUserContext>();

            builder.Services.AddSingleton<ISmsSender, TwilioSmsService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();


            #endregion

            #region Authentication [JWT] & Authorization

            using (var serviceProvider = builder.Services.BuildServiceProvider())
            {
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,
                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                            ValidateLifetime = true
                        };

                        // Allow JWT via query string for SignalR (in WebSockets)
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = ctx => // context
                            {
                                var token = ctx.Request.Query["access_token"];
                                var path = ctx.Request.Path;
                                if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/chatHub"))
                                    ctx.Token = token;

                                return Task.CompletedTask;
                            }
                        };

                    });
            }

            // Authorization: global policy to require authentication by default
            builder.Services.AddAuthorization();

            // Controllers with a global [Authorize] filter
            builder.Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()); // 👈 This is key
            });
            //.AddJsonOptions(options =>
            // {
            //     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            // });




            #endregion


            // Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MessengerClonePolicy", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
                });
            });
            

            // SignalR
            builder.Services.AddSignalR();

            // Auth Mapper
            builder.Services.AddAutoMapper(typeof(AccountProfile).Assembly);

            // Api Explorer Endpoints 
            builder.Services.AddEndpointsApiExplorer();

            // Swagger
            builder.Services.AddSwaggerGen();


            #region Fluent Validation
            var validators = typeof(RegisterDtoValidator).Assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    typeof(IValidator).IsAssignableFrom(t) 
                    && t != typeof(RegisterDtoValidator)
                    )
                .ToList();

            foreach (var validatorType in validators)
            {
                var interfaceType = validatorType
                    .GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                builder.Services.AddScoped(interfaceType, validatorType);
            }

            // Register RegisterDtoValidator manually
            builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();

            #endregion


            var app = builder.Build();

            InitializeDatabase();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("MessengerClonePolicy");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            // Hub for signalR
            app.MapHub<ChatHub>("/chatHub")
                .RequireCors("MessengerClonePolicy").RequireAuthorization();

            app.Run();

            // Initialize DbContext
            void InitializeDatabase()
            {
                using (IServiceScope scope = app.Services.CreateScope())
                {
                    IDbInitializer initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    initializer.Initialize();
                }
            }
        }
    }
}
