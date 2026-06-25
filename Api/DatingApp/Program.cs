using DatingApp.Api.Middlewares;
using DatingApp.Application.Helpers;
using DatingApp.Application.Mapping;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Application.Services.Services;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;
using DatingApp.Infrastructer.Data;
using DatingApp.Infrastructer.Repositories;
using DatingApp.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DatingApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                {
                    var tokenkey = builder.Configuration["tokenkey"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey)),
                        ValidateIssuer=false,
                        ValidateAudience=false,
                        
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                    
                });

            builder.Services.AddAutoMapper(cfg =>
                 cfg.AddProfile<MappingConfig>());

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
               
                options.UseSqlite(builder.Configuration.GetConnectionString("Database"));
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", ploicy => ploicy.RequireRole("Admin"));
            });

            builder.Services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
            }).AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddScoped<LogUserActivity>();
            builder.Services.Configure<CloudinaySetting>(builder.Configuration.GetSection("CloudianrySettings"));
            builder.Services.AddScoped<ITokenService, TokenService>();
         
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
          
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAdminService,AdminService>();
            builder.Services.AddSingleton<PresenceTracker>();

            builder.Services.AddSignalR();
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionsMiddleware>();
            app.UseCors(x =>
            {
                x.AllowAnyHeader().AllowAnyMethod()
                .AllowCredentials().WithOrigins("http://localhost:4200",
                    "https://localhost:4200");
            });

            app.UseAuthentication();
            app.UseAuthorization();
           
            app.UseHttpsRedirection();

            app.MapControllers();

            app.MapHub<PresenceHub>("hubs/presence");
            app.MapHub<MessageHub>("hubs/messages");

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await context.Connections.ExecuteDeleteAsync();
                await SeedData.SeedUsers(userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex,"An Error Occured while seeding the migrations");
            }
            app.Run();
        }
    }
}
