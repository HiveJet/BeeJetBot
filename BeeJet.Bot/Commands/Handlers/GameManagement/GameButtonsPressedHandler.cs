using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;

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
            SocketTextChannel gameChannel = GetGameChannel(Context.Message, gameName);
            if (gameChannel != null)
            {
                await GivePermissionToJoinChannel(Context.User, gameChannel);
            }
        }

        public static async Task GivePermissionToJoinChannel(IUser user, SocketTextChannel gameChannel)
        {
            if (!gameChannel.Users.Any(channelUser => channelUser.Id == user.Id))
            {
                var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
                await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                await gameChannel.SendMessageAsync($"Welcome <@{user.Id}>");
            }
        }

        [ButtonPressedHandler(AddGameCommandHandler.LeaveButtonId)]
        public async Task LeaveGamePressed()
        {
            if (TryGetGameName(Context.Message, out string gameName))
            {
                ITextChannel gameChannel = GetGameChannel(Context.Message, gameName);
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
                    await gameChannel.AddPermissionOverwriteAsync(Context.User, permissionOverrides);
                    await gameChannel.SendMessageAsync($"<@{Context.User.Id}> has left the channel");
                }
            }
            await Context.ComponentInteraction.DeferAsync();
        }

        private static bool TryGetGameName(IUserMessage message, out string gameName)
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

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName)
        {
            var socketMessageChannel = message.Channel as SocketTextChannel;
            var textChannels = socketMessageChannel.Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(channel => channel.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && channel.CategoryId == socketMessageChannel.CategoryId);
            return gameChannel;
        }
    }
}
