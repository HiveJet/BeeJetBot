using BeeJet.Bot;

namespace BeeJet.Web
{
    public class BotService : BackgroundService
    {
        private readonly BeeJetBot _bot;
        private readonly ILogger<BotService> _logger;

        public bool IsRunning { get; private set; }

        public BotService(ILogger<BotService> logger, string clientToken)
        {
            _logger = logger;
            _bot = new BeeJetBot(clientToken);
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