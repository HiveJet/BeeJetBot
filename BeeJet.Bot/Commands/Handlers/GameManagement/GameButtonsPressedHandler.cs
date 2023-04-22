using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using BeeJet.Bot.Extensions;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    public class GameButtonsPressedHandler : ButtonPressedHandler
    {
        [ButtonPressedHandler(AddGameCommandHandler.JointButtonId)]
        public async Task JoinGamePressed()
        {
            if (TryGetGameName(Context.Message, out string gameName))
            {
                await JoinGameAsync(gameName);
            }
            await Context.ComponentInteraction.DeferAsync();
        }

        private async Task JoinGameAsync(string gameName)
        {
            var gameChannel = await Context.Guild.GetTextChannelAsync(gameName, (Context.Channel as SocketTextChannel).Category);
            if (gameChannel != null)
            {
                await Context.User.GivePermissionToJoinChannel((SocketTextChannel)gameChannel, $"Welcome <@{Context.User.Id}>");
            }
        }

        [ButtonPressedHandler(AddGameCommandHandler.LeaveButtonId)]
        public async Task LeaveGamePressed()
        {
            if (TryGetGameName(Context.Message, out string gameName))
            {
                var gameChannel = await Context.Guild.GetTextChannelAsync(gameName, (Context.Channel as SocketTextChannel).Category);
                if (gameChannel != null)
                {
                    await Context.User.RemovePermissionToJoinChannel((SocketTextChannel)gameChannel, $"<@{Context.User.Id}> has left the channel");
                }
            }
            await Context.ComponentInteraction.DeferAsync();
        }

        private bool TryGetGameName(IUserMessage message, out string gameName)
        {
            var regex = new Regex(@"Click to join channel for ([a-zA-Z0-9\s]*)", RegexOptions.Compiled);

            var matching = regex.Match(message.Content);
            if (matching.Success)
            {
                gameName = matching.Groups[1].Value.Trim().Replace(" ", "-");
                return true;
            }
            gameName = null;
            return false;
        }
    }
}
