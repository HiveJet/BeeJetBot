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
        private readonly IButtonContextDb _buttonContextDb;

        public SyncSteamCommandHandler(Services.SteamAPIService steamAPI, IButtonContextDb buttonContextDb)
        {
            _steamAPI = steamAPI;
            _buttonContextDb = buttonContextDb;
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
            List<(string CustomId, ulong ChannelId)> gameIdMapping = new List<(string CustomId, ulong ChannelId)>();
            int buttonIndex = 0;
            foreach (var gameChannel in gamesWithChannel)
            {
                string customId = "join-game-id-" + gameChannel.Name.Replace(" ", "-") + "-" + buttonIndex;
                builder.WithButton(gameChannel.Name + $"({gameChannel.Category.Name})", customId);
                gameIdMapping.Add((customId, gameChannel.Id));
                buttonIndex++;
            }
            
            await Context.SlashCommandInteraction.RespondAsync("Which channels do you want to join?", ephemeral: true, components: builder.Build());
            var response = await Context.SlashCommandInteraction.GetOriginalResponseAsync();
            foreach (var mapping in gameIdMapping)
            {
                _buttonContextDb.CreateNewButtonContext(response.Id, mapping.CustomId, mapping.ChannelId.ToString());
            }
        }

        public void RegisterOptions(SlashCommandBuilder builder)
        {
            builder.AddOption("steamid", ApplicationCommandOptionType.String, "Id of steamuser", isRequired: true);
        }
    }
}
