using BeeJet.Bot.ClientHandlers;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Services;
using BeeJet.Bot.Commands.Sources;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Logging;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BeeJet.Bot
{
    public class BeeJetBot
    {
        public const string BOT_NAME = "BeeJetBot";
        public const string BOT_ADMIN_ROLE_NAME = "BeeJetBotAdmin";

        private readonly string _token;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageHandler _messageHandler;
        private readonly ReactionHandler _reactionHandler;
        private readonly ButtonHandler _buttonHandler;
        private readonly SlashCommandHandler _slashCommandHandler;
        private List<Type> _commandSources;
        private readonly JoinHandler _joinHandler;
        private readonly DiscordLogHandler _discordLogHandler;
        private readonly DiscordLogger _logger;

        public BeeJetBot(BeeJetBotOptions options)
        {
            _token = options.DiscordToken;
            _commandSources = GetCommandSources();
            _logger = new DiscordLogger();


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

            _client.Log += _logger.Log;
            _commandService.Log += _logger.Log;

            var serviceCollection = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_logger)
                .AddSingleton((serviceProvider) => new SteamAPIService(options.SteamAPIKey))
                .AddSingleton((serviceProvider) => new IGDBService(options.IDGBClientId, options.IDGBClientSecret));

            foreach (var commandType in _commandSources)
            {
                serviceCollection.AddSingleton(commandType);
            }
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _messageHandler = new MessageHandler(_client, _commandService, _serviceProvider);
            _reactionHandler = new ReactionHandler(_client, _commandService, _serviceProvider);
            _buttonHandler = new ButtonHandler(_client, _commandService, _serviceProvider);
            _joinHandler = new JoinHandler(_client, _commandService, _serviceProvider);
            _slashCommandHandler = new SlashCommandHandler(_client, _commandService, _serviceProvider, _commandSources.Select(_serviceProvider.GetService).OfType<ICommandSource>().ToList());
            _discordLogHandler = new DiscordLogHandler(_client);
            _logger.BroadcastLogEvent += _discordLogHandler.OnLoggedMessage;
        }

        private List<Type> GetCommandSources()
        {
            var type = typeof(ICommandSource);
            return AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(s => s.GetTypes())
                 .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract).ToList();
        }

        public async Task InstallCommandsAsync()
        {
            await _commandService.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: _serviceProvider);

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += _messageHandler.HandleCommandAsync;
            _client.ReactionAdded += _reactionHandler.ReactionAdded;
            _client.ButtonExecuted += _buttonHandler.ButtonPressed;
            _client.SlashCommandExecuted += _slashCommandHandler.SlashCommandExecuted;
            _client.UserJoined += _joinHandler.UserJoinedAsync;

            await HelpCommands.GenerateHelpCommandAsync(_commandService);

            _client.Ready += OnClientReady;
        }

        private async Task OnClientReady()
        {
            foreach (var commandSource in _commandSources.Select(_serviceProvider.GetService).OfType<ICommandSource>().ToList())
            {
                foreach (var guild in _client.GetBotGuilds())
                {
                    await commandSource.RegisterCommands(guild);
                }
            }
        }

        public async Task LoginAndRun()
        {
            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block until program is closed
            await Task.Delay(-1);
        }
    }
}