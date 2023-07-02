using BeeJet.Bot.Interfaces;

namespace BeeJet.Bot.Commands
{
    public abstract class ButtonPressedHandler : IButtonPressedHandler
    {
        public IButtonPressedContext Context { get; set; }
    }
}
