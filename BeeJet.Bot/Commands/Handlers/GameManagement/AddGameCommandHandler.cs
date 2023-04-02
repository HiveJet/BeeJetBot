using BeeJet.Bot.Commands.Sources;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    internal class AddGameCommandHandler : SlashCommandExecutedHandler
    {
        public AddGameCommandHandler(SocketSlashCommand context) : base(context)
        {
        }

        internal override async Task SlashCommandExecuted()
        {
            var gameName = (string)Context.Data.Options.First().Value;
            var categoryName = "Gaming";//Default name
            var category = Context.Data.Options.FirstOrDefault(b => b.Name == "category");
            if (category != null)
            {
                categoryName = (string)category.Value;

            }
            await AddGameAsync(gameName, categoryName);
        }

        public async Task AddGameAsync(string game, string categoryName)
        {
            ulong roleId = GetAdminRoleId();
            if (!(User as IGuildUser).RoleIds.Contains(roleId))
            {
                await Context.RespondAsync("To add a game you need the role 'BeeJetBotAdmin'", ephemeral: true);
                return;
            }

            var categoryChannel = await GetOrCreateCategoryChannel(categoryName);
            if (categoryChannel is SocketCategoryChannel socketParentCategory && socketParentCategory.Channels.Any(b => b.Name.Equals(game, StringComparison.OrdinalIgnoreCase)))
            {
                await Context.RespondAsync($"This game already has a channel", ephemeral: true);
                return;
            }
            await AddToGameListChannel(game, categoryChannel);

            var channel = await Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"), (properties) => properties.CategoryId = categoryChannel.Id);
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(Guild.EveryoneRole, permissionOverrides);
            await channel.SendMessageAsync($"This is the channel for {game}");


            await Context.RespondAsync($"Channel created", ephemeral: true);
        }

        private async Task<ICategoryChannel> GetOrCreateCategoryChannel(string categoryName)
        {
            ICategoryChannel parentChannel = Guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(b => b.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (parentChannel == null)
            {
                parentChannel = await Guild.CreateCategoryChannelAsync(categoryName);
            }
            return parentChannel;
        }

        private async Task AddToGameListChannel(string game, ICategoryChannel categoryChannel)
        {
            var gameListChannel = await AddOrGetGameListChannel(categoryChannel);
            var builder = new ComponentBuilder().WithButton("Join", GameManagementCommandSource.JointButtonId, ButtonStyle.Success).WithButton("Leave", GameManagementCommandSource.LeaveButtonId, ButtonStyle.Danger);
            var message = await gameListChannel.SendMessageAsync($"Click to join channel for {game}", components: builder.Build());
        }


        private async Task<ITextChannel> AddOrGetGameListChannel(ICategoryChannel categoryChannel)
        {
            if (categoryChannel is not SocketCategoryChannel 
                || (categoryChannel is  SocketCategoryChannel socketCategory &&!socketCategory.Channels.Any(c => c.Name.Equals(GameManagementCommandSource.ChannelName, StringComparison.OrdinalIgnoreCase))))
            {
                var gameListChannel = await Guild.CreateTextChannelAsync(GameManagementCommandSource.ChannelName, (properties)=> properties.CategoryId = categoryChannel.Id);
                var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
                await gameListChannel.AddPermissionOverwriteAsync(Guild.EveryoneRole, permissionOverrides);
                return gameListChannel;
            }
            else
            {
                var textChannels = Guild.Channels.OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(GameManagementCommandSource.ChannelName, StringComparison.OrdinalIgnoreCase) && c.CategoryId == categoryChannel.Id);
            }
        }
    }
}
