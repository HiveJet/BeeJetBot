using Discord;

namespace BeeJet.Bot.Interfaces
{
    public interface IResponseContext
    {
        public IGuild Guild { get; }
        public IChannel Channel { get;  }
        public IUserMessage Message { get;  }
        public IUser User { get;  }
    }
}
