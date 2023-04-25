using BeeJet.Bot;
using BeeJet.Storage.Interfaces;
using BeeJet.Storage.Repositories;
using LiteDB;

namespace BeeJet.Web
{
    public class BotService : BackgroundService
    {
        private readonly BeeJetBot _bot;
        private readonly ILogger<BotService> _logger;

        public bool IsRunning { get; private set; }

        public BotService(ILogger<BotService> logger, IConfiguration configuration, IBeeJetRepository repository)
        {
            _logger = logger;
            var options = new BeeJetBotOptions()
            {
                SteamAPIKey = configuration["STEAM_KEY"],
                IDGBClientId = configuration["IGDB_CLIENTID"],
                IDGBClientSecret = configuration["IGDB_SECRET"],
                DiscordToken = configuration["DISCORD_TOKEN"],
                SteamSignInLink = configuration["SteamSignIn"]
            };

            _bot = new BeeJetBot(options, repository);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(BotService)} starting {nameof(ExecuteAsync)}");
                IsRunning = true;
                await _bot.LoginAndRun();
                IsRunning = false;
                _logger.LogInformation($"{nameof(BotService)} ending {nameof(ExecuteAsync)}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
            }
            finally
            {
                IsRunning = false;
            }            
        }
    }
}