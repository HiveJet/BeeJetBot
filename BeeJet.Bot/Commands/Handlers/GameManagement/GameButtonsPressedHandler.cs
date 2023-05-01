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
            if (TryGetChannelId(Context.Message, AddGameCommandHandler.JointButtonId, out ulong gameChannelId))
            {
                await JoinGameAsync(gameChannelId);
            }
            await Context.ComponentInteraction.DeferAsync();
        }

        private async Task JoinGameAsync(ulong gameChannelId)
        {
            ITextChannel gameChannel = await Context.Client.GetChannelAsync(gameChannelId) as ITextChannel;
            if (gameChannel != null)
            {
                await GivePermissionToJoinChannel(Context.User, gameChannel);
            }
        }

        public static async Task GivePermissionToJoinChannel(IUser user, ITextChannel gameChannel)
        {
            var channelUsers = (await gameChannel.GetUsersAsync().ToListAsync()).SelectMany(b => b);
            if (!channelUsers.Any(channelUser => channelUser.Id == user.Id))
            {
                var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
                await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                await gameChannel.SendMessageAsync($"Welcome <@{user.Id}>");
            }
        }

        [ButtonPressedHandler(AddGameCommandHandler.LeaveButtonId)]
        public async Task LeaveGamePressed()
        {
            if (TryGetChannelId(Context.Message, AddGameCommandHandler.LeaveButtonId, out ulong gameChannelId))
            {
                ITextChannel gameChannel = await Context.Client.GetChannelAsync(gameChannelId) as ITextChannel;
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
                    await gameChannel.AddPermissionOverwriteAsync(Context.User, permissionOverrides);
                    await gameChannel.SendMessageAsync($"<@{Context.User.Id}> has left the channel");
                }
            }
            await Context.ComponentInteraction.DeferAsync();
        }

        private bool TryGetChannelId(IUserMessage message, string customButtonId, out ulong channelId)
        {
            var context = _buttonContextDb.GetButtonContextForMessageIdAndCustomId(message.Id, customButtonId);
            if (context == null)
            {
                channelId = 0;
                return false;
            }
            else
            {
                channelId = ulong.Parse((string)context.HandlerContext);//litedb doesn't support ulong;
                return true;
            }
        }
    }
}
