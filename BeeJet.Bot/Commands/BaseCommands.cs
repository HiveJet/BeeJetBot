using BeeJet.Bot.Logging;
using Discord.Commands;

namespace BeeJet.Bot.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordLogger _logger;

        public BaseCommands(DiscordLogger logger)
        {
            _logger = logger;
        }

        // ~say hello -> hello
        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);

            await _logger.Log($"Executed ECHO: {echo}");
        }

        // ~sample square 20 -> 400
        [Command("square")]
        [Summary("Squares a number.")]
        public async Task SquareAsync([Summary("The number to square.")] int num)
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }
    }
}