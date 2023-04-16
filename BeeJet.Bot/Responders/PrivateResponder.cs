using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Responders
{
    internal class PrivateResponder : IResponder
    {
        private IDiscordInteraction _interactionProvider;

        public PrivateResponder(IDiscordInteraction interactionProvider)
        {
            _interactionProvider = interactionProvider;
        }

        public async Task RespondAsync(string message)
        {
            await _interactionProvider.RespondAsync(message, ephemeral: true);
        }

        public async Task RespondAsync(string message, Embed embeddedInfo)
        {
            await _interactionProvider.RespondAsync(message, ephemeral: true, embed: embeddedInfo);
        }
    }
}
