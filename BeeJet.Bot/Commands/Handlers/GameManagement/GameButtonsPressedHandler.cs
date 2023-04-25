using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using BeeJet.Storage.Interfaces;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    public class GameButtonsPressedHandler : ButtonPressedHandler
    {
        private IButtonContextDb _buttonContextDb;

        public GameButtonsPressedHandler(IButtonContextDb buttonContextDb)
        {
            _buttonContextDb = buttonContextDb;
        }

        [ButtonPressedHandler(AddGameCommandHandler.JointButtonId)]
        public async Task JoinGamePressed()
        {
            if (TryGetGameName(Context.Message, AddGameCommandHandler.JointButtonId, out string gameName))
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
            if (TryGetGameName(Context.Message, AddGameCommandHandler.LeaveButtonId, out string gameName))
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

        private bool TryGetGameName(IUserMessage message, string customButtonId, out string gameName)
        {
            var context = _buttonContextDb.GetButtonContextForMessageIdAndCustomId(message.Id, customButtonId);
            if (context == null)
            {
                gameName = null;
                return false;
            }
            else
            {
                gameName = (string)context.HandlerContext;
                return true;
            }
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName)
        {
            var socketMessageChannel = message.Channel as SocketTextChannel;
            var textChannels = socketMessageChannel.Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(channel => channel.Name.Equals(gameName.Replace(" ", "-"), StringComparison.OrdinalIgnoreCase) && channel.CategoryId == socketMessageChannel.CategoryId);
            return gameChannel;
        }
    }
}
