using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    public class ReactionHandler : BaseClientHandler
    {
        public ReactionHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
        }

        public Task ReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            return Task.CompletedTask;
        }
    }
}
