using BeeJet.Bot.Interfaces;
using Discord;
using Discord.Commands;

namespace BeeJet.Bot.Commands.Handlers
{
    public abstract class BaseHandler
    {
        public IGuildManager GuildManager { get; }
        public abstract IMessageChannel MessageChannel { get; }
        public IGuildChannel GuidChannel => (IGuildChannel)MessageChannel;

        public IGuildUser User { get; private set; }


        public BaseHandler(IGuildManager guildManager, IUser user)
        {
            GuildManager = guildManager;
            Task.Run(() => InitializeAsync(user));
        }

        private async Task InitializeAsync(IUser user)
        {
            User = await GuildManager.GetGuildUserAsync(user);
        }
    }
}