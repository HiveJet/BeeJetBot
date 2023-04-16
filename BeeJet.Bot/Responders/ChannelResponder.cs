using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Responders
{
    internal class ChannelResponder : IResponder
    {
        private IMessageChannel _channel;
        public ChannelResponder(IMessageChannel channel)
        {
            _channel = channel;
        }

        public async Task RespondAsync(string message)
        {
            await _channel.SendMessageAsync(message);
        }

        public async Task RespondAsync(string message, Embed embeddedInfo)
        {
            await _channel.SendMessageAsync(message, embed: embeddedInfo);
        }
    }
}
