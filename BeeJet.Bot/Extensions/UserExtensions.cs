using Discord;

namespace BeeJet.Bot.Extensions
{
    public static class UserExtensions
    {
        public static async Task GivePermissionToJoinChannel(this IUser user, IGuildChannel gameChannel, string message)
        {
            var channelUser = await gameChannel.GetUserAsync(user.Id);
            if (channelUser is null)
            {
                return;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
            await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
            await ((ITextChannel)gameChannel).SendMessageAsync(message);
        }

        public static async Task RemovePermissionToJoinChannelAsync(this IUser user, IGuildChannel gameChannel, string message)
        {
            var channelUser = await gameChannel.GetUserAsync(user.Id);
            if (channelUser is null)
            {
                return;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
            await ((ITextChannel)gameChannel).SendMessageAsync(message);
        }
    }
}
