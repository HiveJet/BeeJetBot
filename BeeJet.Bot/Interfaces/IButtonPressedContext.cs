using Discord;

namespace BeeJet.Bot.Interfaces
{
    public interface IButtonPressedContext : IResponseContext
    {
        IDiscordClient Client { get; }
        IComponentInteraction ComponentInteraction { get; }

        Task Initialize();
    }
}