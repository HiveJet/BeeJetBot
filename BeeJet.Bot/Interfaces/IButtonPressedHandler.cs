using BeeJet.Bot.Interfaces;

namespace BeeJet.Bot.Commands
{
    public interface IButtonPressedHandler
    {
        IButtonPressedContext Context { get; set; }
    }
}