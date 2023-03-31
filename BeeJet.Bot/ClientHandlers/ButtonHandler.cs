using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeJet.Bot.Commands.Handlers;
using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    internal class ButtonHandler : BaseClientHandler
    {
        public ButtonHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
        }

        public async Task ButtonPressed(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case GameManagementHandler.JointButtonId:
                    await GameManagementHandler.JoinGamePressed(component.Message, component.User);
                    break;
                case GameManagementHandler.LeaveButtonId:
                    await GameManagementHandler.LeaveGamePressed(component.Message, component.User);
                    break;
            }
            await component.DeferAsync();
        }
    }
}
