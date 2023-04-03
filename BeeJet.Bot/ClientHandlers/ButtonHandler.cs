using BeeJet.Bot.Commands.Sources;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace BeeJet.Bot.ClientHandlers
{
    internal class ButtonHandler : BaseClientHandler
    {
        private readonly List<(ButtonPressedHandlerAttribute Attribute, MethodInfo Method)> _handlers;

        public ButtonHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
            _handlers = GetHandlers();
        }

        private List<(ButtonPressedHandlerAttribute Attribute, MethodInfo Method)> GetHandlers()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(s => s.GetTypes())
                   .Where(p => !p.IsAbstract).SelectMany(b => b.GetMethods().Select(m => (Attribute: m.GetCustomAttribute<ButtonPressedHandlerAttribute>(), Method: m)).Where(b => b.Attribute != null
                            && b.Method.IsStatic
                            && b.Method.GetParameters().Count() == 1
                            && b.Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(SocketMessageComponent))).ToList();
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
                    handler.Method.Invoke(null, new object[] { component });
                    await component.DeferAsync();
                    return;
                }
            }
        }
    }
}
