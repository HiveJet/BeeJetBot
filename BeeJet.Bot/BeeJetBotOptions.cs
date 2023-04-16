using Microsoft.Extensions.Configuration;

namespace BeeJet.Bot
{
    public class BeeJetBotOptions
    {
        public BeeJetBotOptions()
        {

        }

        public BeeJetBotOptions(IConfiguration configuration)
        {
            SteamAPIKey = configuration["STEAM_KEY"];
            IDGBClientId = configuration["IGDB_CLIENTID"];
            IDGBClientSecret = configuration["IGDB_SECRET"];
            DiscordToken = configuration["DISCORD_TOKEN"];
        }

        public string DiscordToken { get; set; }
        public string SteamAPIKey { get; set; }
        public string IDGBClientId { get; set; }
        public string IDGBClientSecret { get; set; }
    }
}
