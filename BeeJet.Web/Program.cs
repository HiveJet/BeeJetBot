using BeeJet.Bot.Services.SteamAPI;
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
            
            builder.Services.AddSingleton((serviceProvider) => new SteamAPIService(builder.Configuration["STEAM_KEY"]));
            builder.Services.AddSingleton((serviceProvider) => new BotService(serviceProvider.GetService<ILogger<BotService>>(), builder.Configuration["DISCORD_TOKEN"], serviceProvider.GetService<SteamAPIService>()));
            builder.Services.AddHostedService(serviceCollection => serviceCollection.GetRequiredService<BotService>());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}