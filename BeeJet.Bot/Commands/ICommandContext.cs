using Discord.WebSocket;

namespace BeeJet.Bot.Commands
{
    internal interface ICommandContext
    {
        public SocketGuild Guild { get;  }

        public ISocketMessageChannel Channel { get;  }

        public SocketUser User { get;  }
    }
}
