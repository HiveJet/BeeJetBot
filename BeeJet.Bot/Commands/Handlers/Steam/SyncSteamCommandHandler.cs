using BeeJet.Bot.Attributes;
using BeeJet.Bot.Services;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    public class SyncSteamCommandHandler : CommandSource
    {
        private SteamAPIService _steamAPI;

        public SyncSteamCommandHandler(Services.SteamAPIService steamAPI)
        {
            _steamAPI = steamAPI;
        }

        [BeeJetBotSlashCommand("sync-steam", "Sync steam library with channels", nameof(RegisterOptions))]
        public async Task SlashCommandExecuted()
        {
            if (!ulong.TryParse((string)Context.SlashCommandInteraction.Data.Options.First().Value, out ulong steamId))
            {
                await Context.SlashCommandInteraction.RespondAsync("Not a valid steamid", ephemeral: true);
                return;
            }

            var games = await _steamAPI.GetGamesFromSteamUser(steamId);
            var gamesWithChannel = (await Context.Guild.GetChannelsAsync()).OfType<SocketTextChannel>().Where(channel => games.Any(steamGame => steamGame.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)));
            gamesWithChannel = gamesWithChannel.Where(discordChannel => !discordChannel.Users.Any(user => user.Id == Context.SlashCommandInteraction.User.Id));
            if (!gamesWithChannel.Any())
            {
                await Context.SlashCommandInteraction.RespondAsync("No channels to join");
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
            builder.AddOption("steamid", ApplicationCommandOptionType.String, "Id of steamuser", isRequired: true);
        }
    }
}
