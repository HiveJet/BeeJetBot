using BeeJet.Bot.Commands;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace BeeJet.Bot
{
    public class BeeJetBot
    {
        private readonly string? _clientToken = Environment.GetEnvironmentVariable("DISCORD_CLIENT_TOKEN");

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly CommandHandler _commandHandler;
        private readonly Action<string>? _logHandler;
        private readonly IServiceProvider _services;

        //public BeeJetBot(Action<string> logHandler)
        //{
        //    var config = new DiscordSocketConfig()
        //    {
        //        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        //    };

        //    _client = new DiscordSocketClient(config);
        //    _commands = new CommandService();
        //    _commandHandler = new CommandHandler();
        //    _logHandler = logHandler;

        //    _client.Log += Log;

        //    _services = new ServiceCollection()
        //        .AddSingleton(_client)
        //        .AddSingleton(_commands)
        //        .BuildServiceProvider();
        //}

        public BeeJetBot()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _commandHandler = new CommandHandler(_client, _commands);

            _client.Log += Log;

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_commandHandler)
                .BuildServiceProvider();
        }

        private async Task GenerateHelpCommandAsync()
        {
            // Generate help
            foreach (var command in _commands.Commands)
            {
                Console.WriteLine($"We have command '{command.Name}' with summary '{command.Summary}'");
            }

            await _commands.CreateModuleAsync("", async (x) => await BuildHelpCommand(x, _commands.Commands));
        }

        private async Task BuildHelpCommand(ModuleBuilder moduleBuilder, IEnumerable<CommandInfo> _commands)
        {
            moduleBuilder.AddCommand("help",
                async (context, parameters, provider, commandinfo) =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("You called for help? Well! I know the following commands:");

                    foreach (var command in _commands)
                    {
                        sb.AppendLine(ToFriendlyString(command));
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



        public async Task LoginAndRun()
        {
            await _commandHandler.InstallCommandsAsync(_services);

            await _client.LoginAsync(TokenType.Bot, _clientToken);
            await _client.StartAsync();

            // Block until program is closed
            await Task.Delay(-1);
        }

        private Task Log(LogMessage message)
        {
            _logHandler?.Invoke(message.ToString());

            return Task.CompletedTask;
        }

        private static string ToFriendlyString(CommandInfo command)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Command !{command.Name} - {command.Summary}");
            foreach (var parameter in command.Parameters)
                sb.AppendLine($"Parameter: {parameter.Name} {parameter.Type} {(parameter.IsOptional ? " (Optional)" : "")}{(parameter.IsRemainder ? " (Remainder)" : "")}");

            return sb.ToString();
        }
    }
}