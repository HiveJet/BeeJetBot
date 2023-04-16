using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands
{
    public interface ICommandContext
    {
        public SocketGuild Guild { get;  }

        public ISocketMessageChannel Channel { get;  }

        public IUser User { get;  }
    }
}
