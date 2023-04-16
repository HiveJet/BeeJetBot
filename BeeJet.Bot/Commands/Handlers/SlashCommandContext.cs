using BeeJet.Bot.Extensions;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers
{
    public class SlashCommandContext : ICommandContext
    {
        public SocketSlashCommand DiscordContext { get; }

        public SocketGuild Guild { get; set; }

        public ISocketMessageChannel Channel { get; set; }

        public IUser User { get; set; }
        public DiscordSocketClient Client { get; private set; }

        public SlashCommandContext(SocketSlashCommand context)
        {
            DiscordContext = context;
            Channel = DiscordContext.Channel;
            User = DiscordContext.User;
        }

        public virtual async Task Initialize(DiscordSocketClient client)
        {
            if (DiscordContext.GuildId.HasValue)
            {
                Guild = client.GetGuild(DiscordContext.GuildId.Value);
            }
            await Guild.AddAdminRoleIfNeeded();
            Client = client;
        }
    }
}
