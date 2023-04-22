using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Extensions
{
    public static class UserExtensions
    {
        public static async Task GivePermissionToJoinChannel(this IUser user, SocketTextChannel gameChannel, string message)
        {
            if (gameChannel.Users.Any(channelUser => channelUser.Id == user.Id))
            {
                return;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
            await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
            await gameChannel.SendMessageAsync(message);
        }

        public static async Task RemovePermissionToJoinChannel(this IUser user, SocketTextChannel gameChannel, string message)
        {
            if (gameChannel.Users.Any(channelUser => channelUser.Id == user.Id))
            {
                return;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
            await gameChannel.SendMessageAsync(message);
        }
    }
}
