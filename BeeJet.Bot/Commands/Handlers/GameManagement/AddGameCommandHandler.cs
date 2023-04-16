using BeeJet.Bot.Attributes;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Services;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    internal class AddGameCommandHandler : ICommandSource
    {
        internal static readonly string ChannelName = "Game-channels";
        internal const string JointButtonId = "join-game-id";
        internal const string LeaveButtonId = "leave-game-id";
        private readonly IGDBService _igdbService;

        public AddGameCommandHandler(Services.IGDBService igdbService)
        {
            _igdbService = igdbService;
        }

        public void RegisterOptions(SlashCommandBuilder builder)
        {
            builder.AddOption("game", ApplicationCommandOptionType.String, "The name of the game", isRequired: true);
            builder.AddOption("category", ApplicationCommandOptionType.String, "Add to category", isRequired: false);
        }

        [BeeJetBotSlashCommand("add-game", "Add game channel", nameof(RegisterOptions))]
        public async Task SlashCommandExecuted(SlashCommandContext context)
        {
            var gameName = (string)context.DiscordContext.Data.Options.First().Value;
            var categoryName = "Gaming";//Default name
            var category = context.DiscordContext.Data.Options.FirstOrDefault(b => b.Name == "category");
            if (category != null)
            {
                categoryName = (string)category.Value;

            }
            await AddGameAsync(gameName, categoryName, context);
        }

        public async Task AddGameAsync(string game, string categoryName, SlashCommandContext context)
        {
            ulong roleId = context.Guild.GetAdminRoleId();
            if (!(context.User as IGuildUser).RoleIds.Contains(roleId))
            {
                await context.DiscordContext.RespondAsync($"To add a game you need the role '{BeeJetBot.BOT_ADMIN_ROLE_NAME}'", ephemeral: true);
                return;
            }

            var categoryChannel = await GetOrCreateCategoryChannelAsync(categoryName, context);
            var gameInfo = await _igdbService.GetGameInfoAsync(game);
            if (gameInfo != null)
            {
                game = gameInfo.Name;
            }
            if (categoryChannel is SocketCategoryChannel socketParentCategory && socketParentCategory.Channels.Any(b => b.Name.Equals(game, StringComparison.OrdinalIgnoreCase) || b.Name.Replace(" ", "-").Equals(game.Replace(" ", "-"), StringComparison.OrdinalIgnoreCase)))
            {
                await context.DiscordContext.RespondAsync($"This game already has a channel", ephemeral: true);
                return;
            }
            var gameInfoEmbed = CreateGameInfoEmbed(gameInfo);
            await AddToGameListChannelAsync(game, categoryChannel, gameInfoEmbed, context);

            var channel = await context.Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"), (properties) => properties.CategoryId = categoryChannel.Id);
            await context.DiscordContext.RespondAsync($"Channel created", ephemeral: true);

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, permissionOverrides);
            var message = await channel.SendMessageAsync($"This is the channel for {game}", embed: gameInfoEmbed?.Build());

        }

        private async Task<ICategoryChannel> GetOrCreateCategoryChannelAsync(string categoryName, SlashCommandContext context)
        {
            ICategoryChannel parentChannel = context.Guild.Channels.OfType<SocketCategoryChannel>().FirstOrDefault(b => b.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (parentChannel == null)
            {
                parentChannel = await context.Guild.CreateCategoryChannelAsync(categoryName);
            }
            return parentChannel;
        }

        private async Task AddToGameListChannelAsync(string game, ICategoryChannel categoryChannel, EmbedBuilder gameInfoEmbed, SlashCommandContext context)
        {
            var gameListChannel = await AddOrGetGameListChannelAsync(categoryChannel, context);
            var builder = new ComponentBuilder()
                .WithButton("Join", JointButtonId, ButtonStyle.Success)
                .WithButton("Leave", LeaveButtonId, ButtonStyle.Danger);

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

        private async Task<ITextChannel> AddOrGetGameListChannelAsync(ICategoryChannel categoryChannel, SlashCommandContext context)
        {
            if (categoryChannel is not SocketCategoryChannel
                || (categoryChannel is SocketCategoryChannel socketCategory && !socketCategory.Channels.Any(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase))))
            {
                var gameListChannel = await context.Guild.CreateTextChannelAsync(ChannelName, (properties) => properties.CategoryId = categoryChannel.Id);
                var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
                await gameListChannel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, permissionOverrides);
                return gameListChannel;
            }
            else
            {
                var textChannels = context.Guild.Channels.OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase) && c.CategoryId == categoryChannel.Id);
            }
        }
    }
}
