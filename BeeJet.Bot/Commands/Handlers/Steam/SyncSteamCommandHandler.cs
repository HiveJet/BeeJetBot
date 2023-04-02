using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    internal class SyncSteamCommandHandler : SlashCommandExecutedHandler
    {
        public SyncSteamCommandHandler(SocketSlashCommand context, Services.SteamAPIService steamAPI) : base(context)
        {
        }

        internal override async Task SlashCommandExecuted()
        {

        }
    }
}
