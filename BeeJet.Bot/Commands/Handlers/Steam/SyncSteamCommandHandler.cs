﻿using AutoMapper.Execution;
using BeeJet.Bot.Attributes;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Services;
using BeeJet.Storage.Interfaces;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    public class SyncSteamCommandHandler : CommandSource
    {
        private SteamAPIService _steamAPI;
        private readonly ISteamIdDiscordUserDb _steamUserDb;
        private readonly BeeJetBotOptions _beeJetOptions;
        private readonly IButtonContextDb _buttonContextDb;
        public SyncSteamCommandHandler(Services.SteamAPIService steamAPI, ISteamIdDiscordUserDb steamUserDb, BeeJetBotOptions options, IButtonContextDb buttonContextDb)
        {
            _steamAPI = steamAPI;
            _steamUserDb = steamUserDb;
            _beeJetOptions = options;
            _buttonContextDb = buttonContextDb;
        }

        [BeeJetBotSlashCommand("sync-steam", "Sync steam library with channels", nameof(RegisterOptions))]
        public async Task SlashCommandExecuted()
        {
            if (Context.SlashCommandInteraction.Data.Options.Count > 0 && !ulong.TryParse((string)Context.SlashCommandInteraction.Data.Options.First().Value, out ulong steamId))
            {
                await Context.SlashCommandInteraction.RespondEphemeralAsync("Not a valid steamid");
                return;
            }
            else if (await TryGetSteamIdOrChallenge(out steamId) == false)
            {
                return;
            }

            var games = await _steamAPI.GetGamesFromSteamUser(steamId);
            var gamesWithChannel = (await Context.Guild.GetChannelsAsync()).OfType<SocketTextChannel>().Where(channel => games.Any(steamGame => steamGame.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)));
            gamesWithChannel = gamesWithChannel.Where(discordChannel => !discordChannel.Users.Any(user => user.Id == Context.SlashCommandInteraction.User.Id));
            if (!gamesWithChannel.Any())
            {
                await Context.SlashCommandInteraction.RespondEphemeralAsync("No channels to join");
                return;
            }
            var builder = new ComponentBuilder();
            List<(string CustomId, ulong ChannelId)> gameIdMapping = new List<(string CustomId, ulong ChannelId)>();
            int buttonIndex = 0;
            foreach (var gameChannel in gamesWithChannel)
            {
                string customId = "join-game-id-" + gameChannel.Name.Replace(" ", "-") + "-" + buttonIndex;
                builder.WithButton(gameChannel.Name + $"({gameChannel.Category.Name})", customId);
                gameIdMapping.Add((customId, gameChannel.Id));
                buttonIndex++;
            }

            await Context.SlashCommandInteraction.RespondEphemeralAsync("Which channels do you want to join?", components: builder.Build());
            var response = await Context.SlashCommandInteraction.GetOriginalResponseAsync();
            foreach (var mapping in gameIdMapping)
            {
                _buttonContextDb.CreateNewButtonContext(response.Id, mapping.CustomId, mapping.ChannelId.ToString());
            }
        }

        private async Task<bool> TryGetSteamIdOrChallenge(out ulong steamId)
        {
            var steamIdFromDb = _steamUserDb.GetSteamId(Context.User.Id.ToString());
            if (string.IsNullOrWhiteSpace(steamIdFromDb) || !ulong.TryParse(steamIdFromDb, out steamId))
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("You need to link a steam account for this command")
                .AddField("login with url to link steamaccount", _beeJetOptions.SteamSignInLink + Context.User.Id.ToString());

                await Context.SlashCommandInteraction.RespondEphemeralAsync(embed: embed.Build());
                return false;
            }

            return true;
        }

        public void RegisterOptions(SlashCommandBuilder builder)
        {
            builder.AddOption("steamid", ApplicationCommandOptionType.String, "Id of steamuser", isRequired: false);
        }
    }
}
