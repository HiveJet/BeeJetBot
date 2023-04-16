using Discord;

namespace BeeJet.Bot.Interfaces
{
    public interface IResponder
    {
        public Task RespondAsync(string message);
        public Task RespondAsync(string message, Embed embeddedInfo);
    }
}
