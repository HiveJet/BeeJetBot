using BeeJet.Bot.Extensions;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System.Text;

namespace BeeJet.Bot.Commands
{
    internal static class HelpCommands
    {
        internal static async Task GenerateHelpCommandAsync(CommandService commandService)
        {
            // Generate help
            foreach (var command in commandService.Commands)
            {
                Console.WriteLine($"We have command '{command.Name}' with summary '{command.Summary}'");
            }

            await commandService.CreateModuleAsync("", async (x) => await BuildHelpCommand(x, commandService.Commands));
        }

        private static async Task BuildHelpCommand(ModuleBuilder moduleBuilder, IEnumerable<CommandInfo> _commands)
        {
            moduleBuilder.AddCommand("help",
                async (context, parameters, provider, commandinfo) =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("You called for help? Well! I know the following commands:");

                    foreach (var command in _commands)
                    {
                        sb.AppendLine(command.ToFriendlyString());
                    }

                    await context.User.SendMessageAsync(sb.ToString());

                },
                (c) =>
                {
                    c.WithName("help")
                    .AddAliases("?")
                    .WithSummary("Generates list of available commands");
                });


            moduleBuilder
                .WithName("Help module")
                .WithSummary("Help command module");

            await Task.CompletedTask;
        }
    }
}
