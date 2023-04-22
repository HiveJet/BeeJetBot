using BeeJet.Bot;
using BeeJet.Bot.Data;
using BeeJet.Bot.Services;
using LiteDB;

namespace BeeJet.Web
{
    public class BotService : BackgroundService
    {
        private readonly BeeJetBot _bot;
        private readonly ILogger<BotService> _logger;

        public bool IsRunning { get; private set; }

        public BotService(ILogger<BotService> logger, IConfiguration configuration)
        {
            _logger = logger;
            var options = new BeeJetBotOptions()
            {
                SteamAPIKey = configuration["STEAM_KEY"],
                IDGBClientId = configuration["IGDB_CLIENTID"],
                IDGBClientSecret = configuration["IGDB_SECRET"],
                DiscordToken = configuration["DISCORD_TOKEN"]
            };

            var database = new LiteDatabase(configuration.GetConnectionString("LiteDB"));
            _bot = new BeeJetBot(options, database);
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