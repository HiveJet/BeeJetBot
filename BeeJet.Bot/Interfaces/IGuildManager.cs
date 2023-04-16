using Discord;

namespace BeeJet.Bot.Interfaces
{
    public interface IGuildManager
    {
        public Task<bool> ChannelExistsAsync(string channelName);
        public Task<bool> GameListChannelExistsAsync();

        public Task<ITextChannel> GetTextChannelAsync(ulong? channelId);
        public Task<ITextChannel> GetTextChannelAsync(string channelName);
        public Task<ICategoryChannel> GetCategoryChannelAsync(string categoryName);
        public Task<ITextChannel> GetGameListChannelAsync();

        public bool HasAdminRole();
        public Task<IRole> AddAdminRoleAsync();
        public IRole GetAdminRole();
        public bool IsAdmin(IGuildUser user);

        public Task<ITextChannel> CreateGameChannelAsync(string channelName);
        public Task<ITextChannel> CreateGameChannelAsync(string channelName, Action<TextChannelProperties> properties = null);
        public Task<ICategoryChannel> CreateCategoryChannelAsync(string categoryName);
    }
}
