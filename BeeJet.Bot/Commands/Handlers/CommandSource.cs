namespace BeeJet.Bot.Commands.Handlers
{
    public abstract class CommandSource
    {
        public SlashCommandContext Context { get; set; }
    }
}