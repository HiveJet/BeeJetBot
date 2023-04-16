using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Managers;
using Discord;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Extensions;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace BeeJet.Bot.ClientHandlers
{
    public class ButtonHandler : BaseClientHandler
    {
        private readonly List<(ButtonPressedHandlerAttribute Attribute, MethodInfo Method, Type InstanceType)> _handlers;

        public ButtonHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
            _handlers = GetHandlers();
        }

        private List<(ButtonPressedHandlerAttribute Attribute, MethodInfo Method, Type InstanceType)> GetHandlers()
        {
            return GetButtonPressedHandlerTypes().SelectMany(type => type.GetMethods().Select(method => (Attribute: method.GetCustomAttribute<ButtonPressedHandlerAttribute>(), Method: method, InstanceType: type))
                   .Where(method => method.Attribute != null
                            && !method.Method.IsStatic
                            && method.Method.ReturnType == typeof(Task))).ToList();
        }

        public static IEnumerable<Type> GetButtonPressedHandlerTypes()
        {
            var buttonPressedHandlerType = typeof(ButtonPressedHandler);
            return AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(assembly => assembly.GetTypes())
                               .Where(type => !type.IsAbstract && buttonPressedHandlerType.IsAssignableFrom(type));
        }

        public async Task ButtonPressed(SocketMessageComponent component)
        {
            foreach (var handler in _handlers)
            {
                bool customIdMatch = handler.Attribute.StartsWith
                    ? component.Data.CustomId.StartsWith(handler.Attribute.CustomId, StringComparison.OrdinalIgnoreCase)
                    : component.Data.CustomId.Equals(handler.Attribute.CustomId, StringComparison.OrdinalIgnoreCase);
                if (customIdMatch)
                {
                    var context = new ButtonPressedContext(component, _client);
                    using (var scope = _serviceProvider.CreateBeeJetBotResponseScope(context))
                    {
                        var instance = scope.ServiceProvider.GetService(handler.InstanceType) as ButtonPressedHandler;
                        instance.Context = context;
                        await (Task)handler.Method.Invoke(instance, null);
                    }
                    return;
                }
            }
        }
    }
}
