using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Extensions
{
    public static class GuildExtensions
    {
        public async static Task AddAdminRoleIfNeeded(this IGuild guild)
        {
            if (!guild.Roles.Any(b => b.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME))
            {
                await guild.CreateRoleAsync(BeeJetBot.BOT_ADMIN_ROLE_NAME, isMentionable: false);
            }
        }

        public static ulong GetAdminRoleId(this IGuild guild)
        {
            return guild.Roles.FirstOrDefault(b => b.Name == BeeJetBot.BOT_ADMIN_ROLE_NAME).Id;
        }

        public static IEnumerable<SocketGuild> GetBotGuilds(this DiscordSocketClient client)
        {
            return client.Guilds.Where(b => b.Users.Any(u => u.IsBot && u.Username == BeeJetBot.BOT_NAME));
        }

        public static async Task<ICategoryChannel> GetOrCreateCategory(this SocketGuild guild, string categoryName)
        {
            ICategoryChannel parentChannel = guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(b => b.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (parentChannel == null)
            {
                parentChannel = await guild.CreateCategoryChannelAsync(categoryName);
            }
            return parentChannel;
        }
    }
}
