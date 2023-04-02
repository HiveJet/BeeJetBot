using BeeJet.Bot.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    internal class SyncSteamCommandHandler : SlashCommandExecutedHandler
    {
        private SteamAPIService _steamAPI;

        public SyncSteamCommandHandler(SocketSlashCommand context, Services.SteamAPIService steamAPI) : base(context)
        {
            _steamAPI = steamAPI;
        }

        internal override async Task SlashCommandExecuted()
        {
            if (ulong.TryParse((string)Context.Data.Options.First().Value, out ulong steamId))
            {
                var games = await _steamAPI.GetGamesFromSteamUser(steamId);
                var gamesWithChannel = Guild.Channels.OfType<SocketTextChannel>().Where(b => games.Any(c => c.Equals(b.Name, StringComparison.OrdinalIgnoreCase)));
                gamesWithChannel = gamesWithChannel.Where(b => !b.Users.Any(u => u.Id == Context.User.Id));
                if (!gamesWithChannel.Any())
                {
                    await Context.RespondAsync("No channels to join");
                    return;
                }
                var builder = new ComponentBuilder();
                foreach (var game in gamesWithChannel)
                {
                    builder.WithButton(game.Name + $"({game.Category.Name})", "join-game-" + game.Name.Replace(" ", "-") + "-------" + game.Category.Name.Replace(" ", "-"));
                }
                await Context.RespondAsync("Which channels do you want to join?", ephemeral: true, components: builder.Build());
            }
            else
            {
                await Context.RespondAsync("Not a valid steamid", ephemeral: true);
            }
        }
    }
}
