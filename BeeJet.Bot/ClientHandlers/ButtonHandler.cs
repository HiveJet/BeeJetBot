using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Managers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    public class ButtonHandler : BaseClientHandler
    {
        public ButtonHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
        }

        public async Task ButtonPressed(SocketMessageComponent component)
        {
            var guildManager = new GuildManager(((IGuildChannel)component.Message.Channel).Guild);
            var gameHandler = new MessageGameManagementHandler(component.Message, guildManager);
            switch (component.Data.CustomId)
            {
                case MessageGameManagementHandler.JointButtonId:
                    await gameHandler.JoinGamePressed();
                    break;
                case MessageGameManagementHandler.LeaveButtonId:
                    await gameHandler.LeaveGamePressed();
                    break;
            }
            await component.DeferAsync();
        }
    }
}
