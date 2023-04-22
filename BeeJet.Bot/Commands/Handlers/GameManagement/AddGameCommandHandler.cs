﻿using BeeJet.Bot.Attributes;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Services;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    internal class AddGameCommandHandler : CommandSource
    {
        internal static readonly string ChannelName = "Game-channels";
        internal const string JointButtonId = "join-game-id";
        internal const string LeaveButtonId = "leave-game-id";
        private readonly IGDBService _igdbService;

        public AddGameCommandHandler(IGDBService igdbService)
        {
            _igdbService = igdbService;
        }

        public void RegisterOptions(SlashCommandBuilder builder)
        {
            builder.AddOption("game", ApplicationCommandOptionType.String, "The name of the game", isRequired: true);
            builder.AddOption("category", ApplicationCommandOptionType.String, "Add to category", isRequired: false);
        }

        [BeeJetBotSlashCommand("add-game", "Add game channel", nameof(RegisterOptions))]
        public async Task SlashCommandExecuted()
        {
            var gameName = (string)Context.SlashCommandInteraction.Data.Options.First().Value;
            string categoryName = GetCategoryName();
            await AddGameAsync(gameName, categoryName, Context);
        }

        public string GetCategoryName()
        {
            var categoryName = "Gaming";//Default name
            var category = Context.SlashCommandInteraction.Data.Options.FirstOrDefault(commandOptionData => commandOptionData.Name == "category");
            if (category != null)
            {
                categoryName = (string)category.Value;
            }

            return categoryName;
        }

        public async Task AddGameAsync(string game, string categoryName, SlashCommandContext context)
        {
            if (!context.Guild.IsAdmin(context.User as IGuildUser))
            {
                await context.SlashCommandInteraction.RespondAsync($"To add a game you need the role '{BeeJetBot.BOT_ADMIN_ROLE_NAME}'", ephemeral: true);
                return;
            }
            var gameInfo = await _igdbService.GetGameInfoAsync(game.Replace("-", " "));
            if (gameInfo != null)
            {
                game = gameInfo.Name;
            }

            var gameInfoEmbed = CreateGameInfoEmbed(gameInfo);

            var categoryChannel = await context.Guild.GetCategoryChannelAsync(categoryName);
            if (categoryChannel != null)
            {
                if (await categoryChannel.Guild.ChannelExistsAsync(game, categoryChannel))
                {
                    await context.SlashCommandInteraction.RespondAsync($"This game already has a channel", ephemeral: true);
                    return;
                }
            }
            else
            {
                categoryChannel = await context.Guild.CreateCategoryAsync(categoryName);
            }

            await AddToGameListChannelAsync(game, categoryChannel, gameInfoEmbed, context);

            var channel = await context.Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"), (properties) => properties.CategoryId = categoryChannel.Id);
            await context.SlashCommandInteraction.RespondAsync($"Channel created", ephemeral: true);

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, permissionOverrides);
            var message = await channel.SendMessageAsync($"This is the channel for {game}", embed: gameInfoEmbed?.Build());

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
            if (!(await categoryChannel.Guild.GetChannelsAsync()).OfType<INestedChannel>().Any(channel => 
            channel.CategoryId == categoryChannel.Id &&
            channel.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase)))
            {
                var gameListChannel = await context.Guild.CreateTextChannelAsync(ChannelName, (properties) => properties.CategoryId = categoryChannel.Id);
                var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
                await gameListChannel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, permissionOverrides);
                return gameListChannel;
            }
            else
            {
                var textChannels = (await context.Guild.GetChannelsAsync()).OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase) && c.CategoryId == categoryChannel.Id);
            }
        }
    }
}
