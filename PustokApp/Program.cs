using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Services;

namespace PustokApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<PustokAppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("default")));

            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddScoped<EmailService>();

            builder.Services.AddIdentity<Models.AppUser,IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.AllowedForNewUsers = true;  
                
                // Password Reset Token müdd?ti (1 saat)
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
            })
            .AddEntityFrameworkStores<PustokAppDbContext>()
            .AddErrorDescriber<CustomErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<DataProtectorTokenProvider<Models.AppUser>>(TokenOptions.DefaultProvider);

            // Token ya?am müdd?tini t?yin et
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(1)); // 1 saat

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}"
          );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
