using BeeJet.Bot.Commands.Sources;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Services;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    internal class AddGameCommandHandler : SlashCommandExecutedHandler
    {
        private readonly IGDBService _igdbService;

        public AddGameCommandHandler(SocketSlashCommand context, Services.IGDBService igdbService) : base(context)
        {
            _igdbService = igdbService;
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
            ulong roleId = Guild.GetAdminRoleId();
            if (!(User as IGuildUser).RoleIds.Contains(roleId))
            {
                await Context.RespondAsync($"To add a game you need the role '{BeeJetBot.BOT_ADMIN_ROLE_NAME}'", ephemeral: true);
                return;
            }

            var categoryChannel = await GetOrCreateCategoryChannelAsync(categoryName);
            var gameInfo = await _igdbService.GetGameInfoAsync(game);
            if (gameInfo != null)
            {
                game = gameInfo.Name;
            }
            if (categoryChannel is SocketCategoryChannel socketParentCategory && socketParentCategory.Channels.Any(b => b.Name.Equals(game, StringComparison.OrdinalIgnoreCase)))
            {
                await Context.RespondAsync($"This game already has a channel", ephemeral: true);
                return;
            }
            var gameInfoEmbed = CreateGameInfoEmbed(gameInfo);
            await AddToGameListChannelAsync(game, categoryChannel, gameInfoEmbed);

            var channel = await Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"), (properties) => properties.CategoryId = categoryChannel.Id);
            await Context.RespondAsync($"Channel created", ephemeral: true);

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(Guild.EveryoneRole, permissionOverrides);
            var message = await channel.SendMessageAsync($"This is the channel for {game}", embed: gameInfoEmbed.Build());
        }

        private async Task<ICategoryChannel> GetOrCreateCategoryChannelAsync(string categoryName)
        {
            ICategoryChannel parentChannel = Guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(b => b.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (parentChannel == null)
            {
                parentChannel = await Guild.CreateCategoryChannelAsync(categoryName);
            }
            return parentChannel;
        }

        private async Task AddToGameListChannelAsync(string game, ICategoryChannel categoryChannel, EmbedBuilder gameInfoEmbed)
        {
            var gameListChannel = await AddOrGetGameListChannelAsync(categoryChannel);
            var builder = new ComponentBuilder()
                .WithButton("Join", GameManagementCommandSource.JointButtonId, ButtonStyle.Success)
                .WithButton("Leave", GameManagementCommandSource.LeaveButtonId, ButtonStyle.Danger);

            await gameListChannel.SendMessageAsync($"Click to join channel for {game}", embed: gameInfoEmbed?.Build(), components: builder.Build());
        }

        private EmbedBuilder CreateGameInfoEmbed(GameInfo gameInfo)
        {
            if (gameInfo != null)
            {
                var embed = new EmbedBuilder()
                    .WithTitle($"Game info for {gameInfo.Name}")
                    .AddField("Summary", gameInfo.Description);

                if (!string.IsNullOrWhiteSpace(gameInfo.IGDBUrl))
                {
                    embed.WithUrl(gameInfo.IGDBUrl);
                }
                if (!string.IsNullOrWhiteSpace(gameInfo.CoverImage))
                {
                    embed.WithImageUrl("http:" + gameInfo.CoverImage);
                }
                if (gameInfo.Urls.Length > 0)
                {
                    embed.AddField("Links", string.Join(Environment.NewLine, gameInfo.Urls));
                }
                embed.WithFooter(footer => footer.Text = $"Source:IGDB");
                return embed;
            }
            return null;
        }

        private async Task<ITextChannel> AddOrGetGameListChannelAsync(ICategoryChannel categoryChannel)
        {
            if (categoryChannel is not SocketCategoryChannel
                || (categoryChannel is SocketCategoryChannel socketCategory && !socketCategory.Channels.Any(c => c.Name.Equals(GameManagementCommandSource.ChannelName, StringComparison.OrdinalIgnoreCase))))
            {
                var gameListChannel = await Guild.CreateTextChannelAsync(GameManagementCommandSource.ChannelName, (properties) => properties.CategoryId = categoryChannel.Id);
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
