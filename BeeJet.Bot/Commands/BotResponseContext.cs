using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Commands
{
    public abstract class BotResponseContext : IResponseContext
    {
        public IGuild Guild { get; internal set; }

        public IChannel Channel { get; internal set; }
        public IUserMessage Message { get; internal set; }

        public IUser User { get; internal set; }

        public abstract Task Initialize();
    }
}
