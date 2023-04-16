using Discord;

namespace BeeJet.Bot.Commands
{
    public interface IBotResponseContext
    {
        public IGuild Guild { get;  }

        public IChannel Channel { get;  }
        public IUserMessage Message { get; }

        public IUser User { get;  }

        public Task Initialize();
    }
}
