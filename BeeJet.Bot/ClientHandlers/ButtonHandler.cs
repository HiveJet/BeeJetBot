using BeeJet.Bot.Commands.Sources;
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
                case GameManagementCommandSource.JointButtonId:
                    await GameManagementCommandSource.JoinGamePressed(component.Message, component.User);
                    break;
                case GameManagementCommandSource.LeaveButtonId:
                    await GameManagementCommandSource.LeaveGamePressed(component.Message, component.User);
                    break;
            }
            if(component.Data.CustomId.StartsWith("join-game-"))
            {
                await SteamCommandSource.JoinGamePressed(component);
                return;
            }
            await component.DeferAsync();
        }
    }
}
