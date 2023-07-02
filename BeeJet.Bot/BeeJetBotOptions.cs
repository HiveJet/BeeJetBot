using Microsoft.Extensions.Configuration;

namespace BeeJet.Bot
{
    public class BeeJetBotOptions
    {
        public BeeJetBotOptions()
        {

        }

        public string SteamSignInLink { get; set; }
        public string DiscordToken { get; set; }
        public string SteamAPIKey { get; set; }
        public string IDGBClientId { get; set; }
        public string IDGBClientSecret { get; set; }
    }
}
