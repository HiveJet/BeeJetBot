using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using BeeJet.Bot.Commands.Sources;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    public class GameButtonsPressedHandler
    {
        [ButtonPressedHandler(GameManagementCommandSource.JointButtonId)]
        public static async Task JoinGamePressed(SocketMessageComponent component)
        {
            if (TryGetGameName(component.Message, out string gameName))
            {
                await JoinGameAsync(component.Message, component.User, gameName);
            }
        }

        private static async Task JoinGameAsync(IUserMessage message, SocketUser user, string gameName)
        {
            SocketTextChannel gameChannel = GetGameChannel(message, gameName);
            if (gameChannel != null)
            {
                await GivePermissionToJoinChannel(user, gameChannel);
            }
        }

        public static async Task GivePermissionToJoinChannel(SocketUser user, SocketTextChannel gameChannel)
        {
            if (!gameChannel.Users.Any(b => b.Id == user.Id))
            {
                var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
                await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                await gameChannel.SendMessageAsync($"Welcome <@{user.Id}>");
            }
        }

        [ButtonPressedHandler(GameManagementCommandSource.LeaveButtonId)]
        public static async Task LeaveGamePressed(SocketMessageComponent component)
        {
            if (TryGetGameName(component.Message, out string gameName))
            {
                ITextChannel gameChannel = GetGameChannel(component.Message, gameName);
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
                    await gameChannel.AddPermissionOverwriteAsync(component.User, permissionOverrides);
                    await gameChannel.SendMessageAsync($"<@{component.User.Id}> has left the channel");
                }
            }
        }

        private static bool TryGetGameName(IUserMessage message, out string gameName)
        {
            var regex = new Regex(@"Click to join channel for ([a-zA-Z0-9\s]*)");

            var matching = regex.Match(message.Content);
            if (matching.Success)
            {
                gameName = matching.Groups[1].Value.Trim().Replace(" ", "-");
                return true;
            }
            gameName = null;
            return false;
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName)
        {
            var socketMessageChannel = message.Channel as SocketTextChannel;
            var textChannels = socketMessageChannel.Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && b.CategoryId == socketMessageChannel.CategoryId);
            return gameChannel;
        }
    }
}
