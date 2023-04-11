using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    public class MessageHandler : BaseClientHandler
    {
        //TODO:service provider should not be a parameter
        public MessageHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
        }

        public async Task HandleCommandAsync(SocketMessage message)
        {
            // Don't process the command if it was a system message
            if (message is not SocketUserMessage userMessage)
                return;

            // Create a number to track where the prefix ends and the command begins
            int argumentPosition = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(userMessage.HasCharPrefix('!', ref argumentPosition) ||
                userMessage.HasMentionPrefix(_client.CurrentUser, ref argumentPosition)) ||
                userMessage.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, userMessage);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commandService.ExecuteAsync(
                context: context,
                argPos: argumentPosition,
                services: _serviceProvider);
        }
    }
}
