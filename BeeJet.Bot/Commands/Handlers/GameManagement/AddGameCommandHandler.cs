﻿using BeeJet.Bot.Commands.Sources;
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
            var builder = new ComponentBuilder()
                .WithButton("Join", GameManagementCommandSource.JointButtonId, ButtonStyle.Success)
                .WithButton("Leave", GameManagementCommandSource.LeaveButtonId, ButtonStyle.Danger);

            var embed = await CreateGameInfoEmbed(game);

            var message = await gameListChannel.SendMessageAsync($"Click to join channel for {game}", embed: embed?.Build(), components: builder.Build());


            await Context.RespondAsync($"Channel created", ephemeral: true);
        }

        private async Task<EmbedBuilder> CreateGameInfoEmbed(string game)
        {
            var gameInfo = await _igdbService.GetGameInfo(game);
            if (gameInfo != null)
            {
                var embed = new EmbedBuilder()
                    .WithTitle($"Game info for {game}")
                    .AddField("Summary", gameInfo.Description)
                    .WithFooter(footer => footer.Text = "Source:IGDB");
                //.WithUrl("https://example.com")
                //.WithCurrentTimestamp();
                return embed;
            }
            return null;
        }

        private async Task<ITextChannel> AddOrGetGameListChannel(ICategoryChannel categoryChannel)
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
