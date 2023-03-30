using Discord.Commands;

namespace BeeJet.Bot.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        // ~say hello -> hello
        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        // ~sample square 20 -> 400
        [Command("square")]
        [Summary("Squares a number.")]
        public async Task SquareAsync([Summary("The number to square.")] int num)
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }

      /*  [Command("Add game")]
        [Summary("Add gamechannel")]
        public async Task AddGameAsync([Summary("Name of the game")] string game)
        {
            Context.Channel.

            await Context.Channel.SendMessageAsync($"Game added");
        }*/

    }
}