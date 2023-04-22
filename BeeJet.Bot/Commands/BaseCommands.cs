using BeeJet.Bot.Data.Databases;
using BeeJet.Bot.Data.Entities;
using Discord.Commands;

namespace BeeJet.Bot.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IEchoMessageDb _database;

        public BaseCommands(IEchoMessageDb database)
        {
            _database = database;
        }

        // ~say hello -> hello
        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            _database.Add(new EchoMessage() { Message = echo, UserId = Context.User.Id });

            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("echo")]
        [Summary("Echos the last message, if any.")]
        public async Task EchoAsync()
        {
            EchoMessage echo = _database.GetLatestEcho();
            if (echo != null)
            {
                await ReplyAsync($"Latest echo is: '{echo.Message}', said by user {echo.UserId}");
            }
            else
            {
                await ReplyAsync("No echo found.");
            }
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