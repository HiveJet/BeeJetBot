using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Channels;

namespace BeeJet.Bot.Extensions
{
    public static class GuildExtensions
    {
        public async static Task AddAdminRoleIfNeeded(this IGuild guild)
        {
            if (!guild.Roles.Any(role => role.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME))
            {
                await guild.CreateRoleAsync(BeeJetBot.BOT_ADMIN_ROLE_NAME, isMentionable: false);
            }
        }

        public static IRole GetAdminRole(this IGuild guild)
        {
            return guild.Roles.FirstOrDefault(role => role.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME);
        }

        public static IEnumerable<SocketGuild> GetBotGuilds(this DiscordSocketClient client)
        {
            return client.Guilds.Where(guild => guild.Users.Any(u => u.IsBot && u.Username == BeeJetBot.BOT_NAME));
        }

        public static async Task<ICategoryChannel> GetOrCreateCategory(this IGuild guild, string categoryName)
        {
            var channels = await guild.GetChannelsAsync();
            ICategoryChannel parentChannel = channels.OfType<ICategoryChannel>().FirstOrDefault(b => b.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (parentChannel == null)
            {
                parentChannel = await guild.CreateCategoryAsync(categoryName);
            }
            return parentChannel;
        }

        public static async Task<IGuildUser> GetGuildUserAsync(this IGuild guild, IUser user)
        {
            if (user is IGuildUser guildUser)
            {
                return guildUser;
            }
            return await guild.GetUserAsync(user.Id);
        }

        public static async Task<bool> ChannelExistsAsync(this IGuild guild, string channelName)
        {
            var channels = await guild.GetChannelsAsync();
            return channels.Any(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, string channelName)
        {
            var channels = await guild.GetChannelsAsync();
            return channels.OfType<ITextChannel>()
                .FirstOrDefault(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, ulong? channelId)
        {
            if (!channelId.HasValue)
            {
                return null;
            }
            var channels = await guild.GetChannelsAsync();
            return channels.OfType<ITextChannel>()
                .FirstOrDefault(channel => channel.Id.Equals(channelId));
        }

        public static async Task<bool> MainGameListChannelExistsAsync(this IGuild guild)
        {
            return await guild.ChannelExistsAsync(BeeJetBot.BOT_MAIN_CHANNELLIST_NAME);
        }

        public static async Task<ITextChannel> GetMainGameListChannelAsync(this IGuild guild)
        {
            return await guild.GetTextChannelAsync(BeeJetBot.BOT_MAIN_CHANNELLIST_NAME);
        }

        public static async Task<ITextChannel> CreateMainGameListChannel(this IGuild guild)
        {
            return await guild.CreateGameChannelAsync(BeeJetBot.BOT_MAIN_CHANNELLIST_NAME);
        }

        public static async Task<ITextChannel> CreateGameChannelAsync(this IGuild guild, string channelName)
        {
            return await guild.CreateGameChannelAsync(channelName, null);
        }

        public static async Task<ITextChannel> CreateGameChannelAsync(this IGuild guild, string channelName, Action<TextChannelProperties> properties = null)
        {
            var createdChannel = await guild.CreateTextChannelAsync(channelName.Trim().Replace(" ", "-"), properties);
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await createdChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissionOverrides);
            return createdChannel;
        }

        public static bool IsAdmin(this IGuild guild, IGuildUser user)
        {
            var adminRole = guild.GetAdminRole();
            if (adminRole is null)
            {
                return false;
            }
            if (!user.RoleIds.Contains(adminRole.Id))
            {
                return false;
            }
            return true;
        }
    }
}
