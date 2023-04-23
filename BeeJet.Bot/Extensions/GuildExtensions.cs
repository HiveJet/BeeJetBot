using Discord;
using Discord.WebSocket;

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
            var parentChannel = await GetCategoryChannelAsync(guild, categoryName);
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

        public static async Task<bool> ChannelExistsAsync(this IGuild guild, string channelName, string categoryName)
        {
            return (await GetTextChannelAsync(guild, channelName, categoryName)) is not null;
        }

        public static async Task<bool> ChannelExistsAsync(this IGuild guild, string channelName, ICategoryChannel category)
        {
            var channel = await GetTextChannelAsync(guild, channelName, category);
            return channel is not null;
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

        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, string channelName, string categoryName)
        {
            var channels = await guild.GetChannelsAsync();
            var categoryChannel = await GetCategoryChannelAsync(guild, categoryName);
            return channels.OfType<INestedChannel>()
                .FirstOrDefault(channel => channel.CategoryId == categoryChannel.Id && channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase)) as ITextChannel;
        }

        public static async Task<ITextChannel> GetTextChannelAsync(this IGuild guild, string channelName, ICategoryChannel category)
        {
            if(category is null)
            {
                return null;
            }
            var channels = await guild.GetChannelsAsync();
            var channel = channels.OfType<INestedChannel>()
                .FirstOrDefault(channel => channel.CategoryId == category.Id && channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
            return channel as ITextChannel;
        }

        public static async Task<bool> MainGameListChannelExistsAsync(this IGuild guild)
        {
            return await guild.ChannelExistsAsync(BeeJetBot.BOT_MAIN_CHANNELLIST_NAME);
        }

        public static async Task<ITextChannel> GetMainGameListChannelAsync(this IGuild guild)
        {
            return await guild.GetTextChannelAsync(BeeJetBot.BOT_MAIN_CHANNELLIST_NAME);
        }

        public static async Task<ICategoryChannel> GetCategoryChannelAsync(this IGuild guild, ulong? categoryId)
        {
            if(!categoryId.HasValue)
            {
                return null;
            }
            return (await guild.GetChannelsAsync())
                .OfType<ICategoryChannel>()
                .FirstOrDefault(channel => channel.Id.Equals(categoryId.Value));
        }

        public static async Task<ICategoryChannel> GetCategoryChannelAsync(this IGuild guild, string categoryName)
        {
            return (await guild.GetChannelsAsync())
                .OfType<ICategoryChannel>()
                .FirstOrDefault(channel => channel.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
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
