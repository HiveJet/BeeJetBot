using BeeJet.Bot.Commands.Handlers;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;
using System.Threading.Channels;

namespace BeeJet.Bot
{
    public class BeeJetBot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
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

        private readonly string _token;

        public BeeJetBot(string token)
        {
            _token = token;

            var config = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(config);
            _commands = new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });

            _client.Log += Log;
            _commands.Log += Log;

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
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
            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block until program is closed
            await Task.Delay(-1);
        }

        //private Task Log(LogMessage message)
        //{
        //    _logHandler?.Invoke(message.ToString());

        //    return Task.CompletedTask;
        //}

        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

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

        public async Task InstallCommandsAsync()
        {
            await _commands.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: _services);

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            _client.ReactionAdded += ReactionAdded;
            _client.ButtonExecuted += ButtonPressed;
        }

        private async Task ButtonPressed(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case GameManagementHandler.JointButtonId:
                    await GameManagementHandler.JoinGamePressed(component.Message, component.User);
                    break;
                case GameManagementHandler.LeaveButtonId:
                    await GameManagementHandler.LeaveGamePressed(component.Message, component.User);
                    break;
            }
            await component.DeferAsync();
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {

        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            // Don't process the command if it was a system message
            if (message is not SocketUserMessage userMessage)
                return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(userMessage.HasCharPrefix('!', ref argPos) ||
                userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                userMessage.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, userMessage);

            //var searchResult = _commands.Search(context, argPos);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }
}