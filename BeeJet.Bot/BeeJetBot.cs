using BeeJet.Bot.ClientHandlers;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Extensions;
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
        private readonly string _token;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageHandler _messageHandler;
        private readonly ReactionHandler _reactionHandler;
        private readonly ButtonHandler _buttonHandler;

        public BeeJetBot(string token)
        {
            _token = token;

            var config = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All
            };

            _client = new DiscordSocketClient(config);
            _commandService = new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });

            _client.Log += Log;
            _commandService.Log += Log;

            _serviceProvider = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .BuildServiceProvider();

            _messageHandler = new MessageHandler(_client, _commandService, _serviceProvider);
            _reactionHandler = new ReactionHandler(_client, _commandService, _serviceProvider);
            _buttonHandler = new ButtonHandler(_client, _commandService, _serviceProvider);
        }

        public async Task InstallCommandsAsync()
        {
            await _commandService.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: _serviceProvider);

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += _messageHandler.HandleCommandAsync;
            _client.ReactionAdded += _reactionHandler.ReactionAdded;
            _client.ButtonExecuted += _buttonHandler.ButtonPressed;

            await HelpCommands.GenerateHelpCommandAsync(_commandService);
        }

        public async Task LoginAndRun()
        {
            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block until program is closed
            await Task.Delay(-1);
        }

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
    }
}