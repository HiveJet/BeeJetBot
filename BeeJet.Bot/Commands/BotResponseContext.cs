using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Commands
{
    public abstract class BotResponseContext : IResponseContext
    {
        public IGuild Guild { get; protected set; }

        public IChannel Channel { get; protected set; }
        public IUserMessage Message { get; protected set; }

        public IUser User { get; protected set; }

        public abstract Task Initialize();
    }
}
