using BeeJet.Bot.Commands.Handlers;
using Discord.Commands;

namespace BeeJet.Bot.Commands
{
    public class GameCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Add_game")]
        [Summary("Add gamechannel")]
        public async Task AddGameAsync([Summary("Name of the game")] string game)
        {
            await new GameManagementHandler(Context).AddGameAsync(game);
        }
    }
}