using Discord;

namespace BeeJet.Bot.Interfaces
{
    public interface IGuildManager
    {
        public Task<bool> ChannelExistsAsync(string channelName);
        public Task<bool> MainGameListChannelExistsAsync();

        public Task<ITextChannel> GetTextChannelAsync(ulong? channelId);
        public Task<ITextChannel> GetTextChannelAsync(string channelName);
        public Task<ITextChannel> GetMainGameListChannelAsync();
        public Task<ICategoryChannel> GetCategoryChannelAsync(string categoryName);

        public bool HasAdminRole();
        public Task<IRole> AddAdminRoleAsync();
        public IRole GetAdminRole();
        public bool IsAdmin(IGuildUser user);
        public Task<IGuildUser> GetGuildUserAsync(IUser user);

        public Task<ITextChannel> CreateMainGameListChannel();
        public Task<ITextChannel> CreateGameChannelAsync(string channelName);
        public Task<ITextChannel> CreateGameChannelAsync(string channelName, Action<TextChannelProperties> properties = null);
        public Task<ICategoryChannel> CreateCategoryChannelAsync(string categoryName);
    }
}
