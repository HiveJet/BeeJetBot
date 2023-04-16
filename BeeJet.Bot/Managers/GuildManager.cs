using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Managers
{
    public class GuildManager : IGuildManager
    {
        private const string _adminRoleName = "BeeJetBotAdmin";
        private const string _gameListChannelName = "Game-channels";

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

        public async Task<bool> GameListChannelExistsAsync()
        {
            return await ChannelExistsAsync(_gameListChannelName);
        }

        public async Task<ITextChannel> GetTextChannelAsync(string channelName)
        {
            var channels = await GetChannelsAsync();
            return channels.OfType<ITextChannel>()
                .FirstOrDefault(channel => channel.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ITextChannel> GetGameListChannelAsync()
        {
            return await GetTextChannelAsync(_gameListChannelName);
        }

        protected async Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync()
        {
            return await _guild.GetChannelsAsync();
        }

        public bool HasAdminRole()
        {
            return _guild.Roles.Any(role => role.Name == _adminRoleName);
        }

        public async Task<IRole> AddAdminRoleAsync()
        {
            return await _guild.CreateRoleAsync(_adminRoleName, isMentionable: false);
        }

        public IRole GetAdminRole()
        {
            return _guild.Roles.FirstOrDefault(role => role.Name == _adminRoleName);
        }

        public async Task<ITextChannel> CreateGameChannelAsync(string channelName)
        {
            var createdChannel = await _guild.CreateTextChannelAsync(channelName.Trim().Replace(" ", "-"));
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await createdChannel.AddPermissionOverwriteAsync(_guild.EveryoneRole, permissionOverrides);
            return createdChannel;
        }

        public async Task<ITextChannel> CreateGameListChannelAsync()
        {
            var createdChannel = await _guild.CreateTextChannelAsync(_gameListChannelName);
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
    }
}