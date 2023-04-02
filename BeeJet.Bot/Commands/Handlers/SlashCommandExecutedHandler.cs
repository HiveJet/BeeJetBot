using BeeJet.Bot.Extensions;
using Discord;
using Discord.WebSocket;
using System.Threading.Channels;

namespace BeeJet.Bot.Commands.Handlers
{
    internal abstract class SlashCommandExecutedHandler : ICommandHandler
    {
        public SocketSlashCommand Context { get; }

        public SocketGuild Guild { get; set; }

        public ISocketMessageChannel Channel { get; set; }

        public SocketUser User { get; set; }
        public DiscordSocketClient Client { get; private set; }

        public SlashCommandExecutedHandler(SocketSlashCommand context)
        {
            Context = context;
            Channel = Context.Channel;
            User = Context.User;
        }

        public virtual async Task Initialize(DiscordSocketClient client)
        {
            if (Context.GuildId.HasValue)
            {
                Guild = client.GetGuild(Context.GuildId.Value);
            }
            await Guild.AddAdminRoleIfNeeded();
            Client = client;
        }

        internal abstract Task SlashCommandExecuted();
    }
}
