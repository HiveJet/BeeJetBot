using BeeJet.Bot.Interfaces;
using Discord;
using System;

namespace BeeJet.Bot.Managers
{
    public class GuildManager : IGuildManager
    {
        private const string _mainGameCategoryName = "Game-channels";

        public IGuild _guild { get; }

        public GuildManager(IGuild guild)
        {
            _guild = guild;
        }

        public async Task<bool> ChannelExistsAsync(string channelName)
        {
            var channels = await GetChannelsAsync();
            return channels.Any(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ITextChannel> GetTextChannelAsync(string channelName)
        {
            var channels = await GetChannelsAsync();
            return channels.OfType<ITextChannel>()
                .FirstOrDefault(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ITextChannel> GetTextChannelAsync(ulong? channelId)
        {
            if (!channelId.HasValue)
            {
                return null;
            }
            var channels = await GetChannelsAsync();
            return channels.OfType<ITextChannel>()
                .FirstOrDefault(channel => channel.Id.Equals(channelId));
        }

        public async Task<ICategoryChannel> GetCategoryChannelAsync(string categoryName)
        {
            var channels = await GetChannelsAsync();
            return channels.OfType<ICategoryChannel>()
                .FirstOrDefault(channel => channel.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> MainGameListChannelExistsAsync()
        {
            return await ChannelExistsAsync(_mainGameCategoryName);
        }

        public async Task<ITextChannel> GetMainGameListChannelAsync()
        {
            return await GetTextChannelAsync(_mainGameCategoryName);
        }

        protected async Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync()
        {
            return await _guild.GetChannelsAsync();
        }

        public bool HasAdminRole()
        {
            return _guild.Roles.Any(role => role.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME);
        }

        public async Task<IRole> AddAdminRoleAsync()
        {
            return await _guild.CreateRoleAsync(BeeJetBot.BOT_ADMIN_ROLE_NAME, isMentionable: false);
        }

        public IRole GetAdminRole()
        {
            return _guild.Roles.FirstOrDefault(role => role.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME);
        }

        public async Task<ITextChannel> CreateMainGameListChannel()
        {
            return await CreateGameChannelAsync(_mainGameCategoryName);
        }

        public async Task<ITextChannel> CreateGameChannelAsync(string channelName)
        {
            return await CreateGameChannelAsync(channelName, null);
        }

        public async Task<ITextChannel> CreateGameChannelAsync(string channelName, Action<TextChannelProperties> properties = null)
        {
            var createdChannel = await _guild.CreateTextChannelAsync(channelName.Trim().Replace(" ", "-"), properties);
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await createdChannel.AddPermissionOverwriteAsync(_guild.EveryoneRole, permissionOverrides);
            return createdChannel;
        }

        public async Task<ICategoryChannel> CreateCategoryChannelAsync(string categoryName)
        {
            return await _guild.CreateCategoryAsync(categoryName);
        }

        public async Task<ITextChannel> CreateCategoryChannelAsync()
        {
            var createdChannel = await _guild.CreateTextChannelAsync(_mainGameCategoryName);
            var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
            await createdChannel.AddPermissionOverwriteAsync(_guild.EveryoneRole, permissionOverrides);
            return createdChannel;
        }

        public bool IsAdmin(IGuildUser user)
        {
            var adminRole = GetAdminRole();
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

        public async Task<IGuildUser> GetGuildUserAsync(IUser user)
        {
            if (user is IGuildUser guildUser)
            {
                return guildUser;
            }
            return await _guild.GetUserAsync(user.Id);
        }
    }
}