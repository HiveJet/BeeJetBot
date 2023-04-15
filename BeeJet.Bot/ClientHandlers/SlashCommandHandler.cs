using BeeJet.Bot.Attributes;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Extensions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace BeeJet.Bot.ClientHandlers
{
    internal class SlashCommandHandler : BaseClientHandler
    {
        private readonly List<(Type ClassType, MethodInfo Method, string CommandName)> _commandMethods;

        public SlashCommandHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
            _commandMethods = GetCommandMethods();
        }
        private List<(Type ClassType, MethodInfo Method, string CommandName)> GetCommandMethods()
        {
            var commandSources = GetCommandSourceTypes();
            return commandSources.SelectMany(c => c.GetMethods()
                                                .Where(m =>
                                                m.GetCustomAttribute<BeeJetBotSlashCommandAttribute>() != null
                                                ).Select(m => (ClassType: c, Method: m, CommandName: m.GetCustomAttribute<BeeJetBotSlashCommandAttribute>().CommandName))).ToList();
        }

        public static IEnumerable<Type> GetCommandSourceTypes()
        {
            var type = typeof(CommandSource);
            return AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(s => s.GetTypes())
                               .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);
        }

        internal async Task SlashCommandExecuted(SocketSlashCommand slashCommandArguments)
        {
            var commandHandler = _commandMethods.FirstOrDefault(b => b.CommandName.Equals(slashCommandArguments.CommandName, StringComparison.OrdinalIgnoreCase));
            if (commandHandler.ClassType != null)
            {
                var context = new SlashCommandContext(slashCommandArguments, _client);
                using (var scope = _serviceProvider.CreateBeeJetBotResponseScope(context))
                {
                    var handlerInstance = scope.ServiceProvider.GetService(commandHandler.ClassType) as CommandSource;
                    if (handlerInstance != null)
                    {
                        handlerInstance.Context = context;
                        await handlerInstance.Context.Initialize();
                        commandHandler.Method.Invoke(handlerInstance, null);
                    }
                }
            }
        }

        internal async Task OnClientReadyAsync()
        {
            foreach (var commandSource in _commandMethods)
            {
                SlashCommandBuilder guildCommand = CreateCommand(commandSource);
                await AddCommandToGuilds(guildCommand);
            }
        }

        private async Task AddCommandToGuilds(SlashCommandBuilder guildCommand)
        {
            foreach (var guild in _client.GetBotGuilds())
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
        }

        private SlashCommandBuilder CreateCommand((Type ClassType, MethodInfo Method, string CommandName) commandSource)
        {
            var attribute = commandSource.Method.GetCustomAttribute<BeeJetBotSlashCommandAttribute>();
            SlashCommandBuilder builder = new SlashCommandBuilder();
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName(attribute.CommandName);
            guildCommand.WithDescription(attribute.Description);
            if (!string.IsNullOrWhiteSpace(attribute.BuilderMethod))
            {
                var builderMethod = commandSource.ClassType.GetMethods().FirstOrDefault(b => b.Name == attribute.BuilderMethod && b.GetParameters().Length == 1 && b.GetParameters()[0].ParameterType == typeof(SlashCommandBuilder));
                if (builderMethod != null)
                {
                    builderMethod.Invoke(_serviceProvider.GetService(commandSource.ClassType), new object[] { guildCommand });
                }
            }

            return guildCommand;
        }
    }
}
