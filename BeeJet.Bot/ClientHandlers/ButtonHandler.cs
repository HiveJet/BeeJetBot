using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Managers;
using Discord;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Commands.Sources;
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
            return GetButtonPressedHandlerTypes().SelectMany(type => type.GetMethods().Select(m => (Attribute: m.GetCustomAttribute<ButtonPressedHandlerAttribute>(), Method: m, InstanceType: type))
                   .Where(b => b.Attribute != null
                            && !b.Method.IsStatic
                            && b.Method.GetParameters().Count() == 1
                            && b.Method.ReturnType == typeof(Task)
                            && b.Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(SocketMessageComponent))).ToList();
        }

        public static IEnumerable<Type> GetButtonPressedHandlerTypes()
        {
            var interfaceType = typeof(IButtonPressedHandler);
            return AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(s => s.GetTypes())
                               .Where(type => !type.IsAbstract && interfaceType.IsAssignableFrom(type));
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
                    var instance = _serviceProvider.GetService(handler.InstanceType);
                    await (Task)handler.Method.Invoke(instance, new object[] { component });
                    await component.DeferAsync();
                    return;
                }
            }
        }
    }
}
