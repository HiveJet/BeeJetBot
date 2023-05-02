using BeeJet.Bot.Services;
using BeeJet.Storage.Interfaces;
using BeeJet.Storage.Repositories;
using LiteDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BeeJet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddSingleton<ILiteDatabase>(new LiteDatabase(builder.Configuration.GetConnectionString("LiteDB")));
            builder.Services.AddSingleton<IBeeJetRepository, BeeJetRepository>();
            builder.Services.AddSingleton<BotService>();
            builder.Services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<BotService>());
            builder.Services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/signin";
               options.LogoutPath = "/signout";
               options.AccessDeniedPath = "/";
               options.ExpireTimeSpan = TimeSpan.FromDays(7);
               /*   options.Events.OnSignedIn = ValidationHelper.SignIn;
                  options.Events.OnValidatePrincipal = ValidationHelper.Validate;*/
           })
           .AddSteam();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.MapControllers();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}