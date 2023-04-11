using BeeJet.Bot.Interfaces;
using Discord;
using Discord.Commands;

namespace BeeJet.Bot.Commands.Handlers
{
    public abstract class BaseHandler
    {
        public IGuildManager GuildManager { get; }
        public abstract IMessageChannel MessageChannel { get; }// => Context is not null ? Context.Channel : UserMessage?.Channel;
        public IGuildChannel GuidChannel => (IGuildChannel)MessageChannel;

        public abstract IGuildUser User { get; }// => (IGuildUser)(Context is not null ? Context.User : (IGuildUser)UserMessage.Author);


        public BaseHandler(IGuildManager guildManager)
        {
            GuildManager = guildManager;
        }
    }
}