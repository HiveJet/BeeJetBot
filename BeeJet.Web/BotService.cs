using BeeJet.Bot;

namespace BeeJet.Web
{
    public class BotService : BackgroundService
    {
        private readonly BeeJetBot _bot;
        private readonly ILog

        public BotService(ILogger<BotService> logger)
        {
            _bot = new BeeJetBot();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bot.LoginAndRun();
        }
    }
}