using BeeJet.Bot.Commands.Sources;
using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    internal class SlashCommandHandler : BaseClientHandler
    {
        private readonly List<ICommandSource> _commandSources;

        public SlashCommandHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider, List<ICommandSource> commandSources)
            : base(client, service, serviceProvider)
        {
            _commandSources = commandSources;
        }

        internal async Task SlashCommandExecuted(SocketSlashCommand slashCommandArguments)
        {
            var commandHandler = _commandSources.FirstOrDefault(b => b.GetCommandNames().Contains(slashCommandArguments.CommandName));
            if (commandHandler != null)
            {
                var handler = commandHandler.GetCommandHandler(slashCommandArguments.CommandName, slashCommandArguments);
                if (handler != null)
                {
                    await handler.Initialize(_client);
                    await handler.SlashCommandExecuted();
                }
            }
          
        }
    }
}
