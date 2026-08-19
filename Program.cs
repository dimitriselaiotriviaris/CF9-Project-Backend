using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CF9Project.Data;
using CF9Project.Repositories;
using CF9Project.Security;
using CF9Project.Services;
using Serilog;

namespace CF9Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connString = builder.Configuration.GetConnectionString("DevConnection");
            builder.Services.AddDbContext<CF9ProjectContext>(options =>
            {
                options.UseSqlServer(connString);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            });

            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();
            builder.Services.AddRepositories();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IGamerService, GamerService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<Configuration.MapperConfig>());
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularDevelopment", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = "/User/Login";
                   options.AccessDeniedPath = "/Home/AccessDenied";
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.SlidingExpiration = true;   // reset timeout, 30 min of idle

                   // Important for Angular -> backend cookie auth
                   options.Cookie.HttpOnly = true;
                   options.Cookie.SameSite = SameSiteMode.None;
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                   // API calls must receive HTTP 401/403 instead of an HTML redirect.
                   options.Events.OnRedirectToLogin = context =>
                   {
                       if (context.Request.Path.StartsWithSegments("/api"))
                       {
                           context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           return Task.CompletedTask;
                       }

                       context.Response.Redirect(context.RedirectUri);
                       return Task.CompletedTask;
                   };

                   options.Events.OnRedirectToAccessDenied = context =>
                   {
                       if (context.Request.Path.StartsWithSegments("/api"))
                       {
                           context.Response.StatusCode = StatusCodes.Status403Forbidden;
                           return Task.CompletedTask;
                       }

                       context.Response.Redirect(context.RedirectUri);
                       return Task.CompletedTask;
                   };
               });

            builder.Services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
            .AddPolicy("CanInsertCompany", policy =>
                policy.RequireClaim("Capability", "INSERT_COMPANY"))
            .AddPolicy("CanViewCompanies", policy =>
                policy.RequireClaim("Capability", "VIEW_COMPANIES"))
            .AddPolicy("CanDeleteGamer", policy =>
                policy.RequireClaim("Capability", "DELETE_GAMER"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection(); 
            app.UseRouting();

            app.UseCors("AngularDevelopment");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets().AllowAnonymous();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
