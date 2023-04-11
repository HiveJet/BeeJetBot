using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Managers;
using Discord.Commands;

namespace BeeJet.Bot.Commands
{
    public class GameCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Add_game")]
        [Summary("Add gamechannel")]
        public async Task AddGameAsync([Summary("Name of the game")] string game)
        {
            var guildManager = new GuildManager(Context.Guild);
            await new ContextGameManagementHandler(Context, guildManager).AddGameAsync(game);
        }
    }
}