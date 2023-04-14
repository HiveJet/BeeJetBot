using BeeJet.Bot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BeeJet.Bot.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static AsyncServiceScope CreateBeeJetBotResponseScope(this IServiceProvider serviceProvider, IBotReponseContext context)
        {
            var scope = serviceProvider.CreateAsyncScope();
            //TODO: Fill context in all general scope DI services
            return scope;
        }
    }
}
