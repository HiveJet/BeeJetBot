using BeeJet.Bot.Extensions;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands
{
    public class SlashCommandContext : IBotResponseContext
    {
        public ISlashCommandInteraction SlashCommandInteraction { get; }

        public IGuild Guild { get; set; }

        public IChannel Channel { get; set; }

        public IUserMessage Message { get; }

        public IUser User { get; set; }
        public IDiscordClient Client { get; private set; }

        public SlashCommandContext()
        {
        }

        public SlashCommandContext(SocketSlashCommand context, IDiscordClient client)
        {
            SlashCommandInteraction = context;
            User = SlashCommandInteraction.User;
            Message = null;
            Client = client;
        }

        public virtual async Task Initialize()
        {
            if (SlashCommandInteraction.GuildId.HasValue)
            {
                Guild = await Client.GetGuildAsync(SlashCommandInteraction.GuildId.Value);
            }
            if (SlashCommandInteraction.ChannelId.HasValue)
            {
                Channel = await Client.GetChannelAsync(SlashCommandInteraction.ChannelId.Value);
            }
            await Guild.AddAdminRoleIfNeeded();
        }
    }
}
