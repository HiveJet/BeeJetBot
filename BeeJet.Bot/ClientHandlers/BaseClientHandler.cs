using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    internal abstract class BaseClientHandler
    {
        protected readonly DiscordSocketClient _client;
        protected readonly CommandService _commandService;
        protected readonly IServiceProvider _serviceProvider;

        public BaseClientHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = service;
            _serviceProvider = serviceProvider;
        }
    }
}
