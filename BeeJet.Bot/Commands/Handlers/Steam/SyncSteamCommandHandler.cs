using AutoMapper.Execution;
using BeeJet.Bot.Attributes;
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

        public SyncSteamCommandHandler(Services.SteamAPIService steamAPI, ISteamIdDiscordUserDb steamUserDb, BeeJetBotOptions options)
        {
            _steamAPI = steamAPI;
            _steamUserDb = steamUserDb;
            _beeJetOptions = options;
        }

        [BeeJetBotSlashCommand("sync-steam", "Sync steam library with channels", nameof(RegisterOptions))]
        public async Task SlashCommandExecuted()
        {
            if (Context.SlashCommandInteraction.Data.Options.Count > 0 && !ulong.TryParse((string)Context.SlashCommandInteraction.Data.Options.First().Value, out ulong steamId))
            {
                await Context.SlashCommandInteraction.RespondAsync("Not a valid steamid", ephemeral: true);
                return;
            }
            else
            {
                var steamIdFromDb = _steamUserDb.GetSteamId(Context.User.Id.ToString());
                if (string.IsNullOrWhiteSpace(steamIdFromDb) || !ulong.TryParse(steamIdFromDb, out steamId))
                {
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithTitle("You need to link a steam account for this command")
                    .AddField("login with url to link steamaccount", _beeJetOptions.SteamSignInLink + Context.User.Id.ToString());

                    await Context.SlashCommandInteraction.RespondAsync(ephemeral: true, embed: embed.Build());
                    return;
                }
            }

            var games = await _steamAPI.GetGamesFromSteamUser(steamId);
            var gamesWithChannel = (await Context.Guild.GetChannelsAsync()).OfType<SocketTextChannel>().Where(channel => games.Any(steamGame => steamGame.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)));
            gamesWithChannel = gamesWithChannel.Where(discordChannel => !discordChannel.Users.Any(user => user.Id == Context.SlashCommandInteraction.User.Id));
            if (!gamesWithChannel.Any())
            {
                await Context.SlashCommandInteraction.RespondAsync("No channels to join", ephemeral: true);
                return;
            }
            var builder = new ComponentBuilder();
            foreach (var game in gamesWithChannel)
            {
                builder.WithButton(game.Name + $"({game.Category.Name})", "join-game-id-" + game.Name.Replace(" ", "-") + "-------" + game.Category.Name.Replace(" ", "-"));
            }
            await Context.SlashCommandInteraction.RespondAsync("Which channels do you want to join?", ephemeral: true, components: builder.Build());
        }

        public void RegisterOptions(SlashCommandBuilder builder)
        {
            builder.AddOption("steamid", ApplicationCommandOptionType.String, "Id of steamuser", isRequired: false);
        }
    }
}
