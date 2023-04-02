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
        }

        internal abstract Task SlashCommandExecuted();


        protected async Task AddAdminRoleIfNeeded()
        {
            if (!Guild.Roles.Any(b => b.Name == "BeeJetBotAdmin"))
            {
                await Guild.CreateRoleAsync("BeeJetBotAdmin", isMentionable: false);
            }
        }

        protected ulong GetAdminRoleId()
        {
            return Guild.Roles.FirstOrDefault(b => b.Name == "BeeJetBotAdmin").Id;
        }
    }
}
