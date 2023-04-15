namespace BeeJet.Bot.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BeeJetBotSlashCommandAttribute : Attribute
    {
        public BeeJetBotSlashCommandAttribute(string commandName, string description, string builderMethod = null)
        {
            CommandName = commandName;
            Description = description;
            BuilderMethod = builderMethod;
        }
        public string CommandName { get; }
        public string Description { get; }
        public string BuilderMethod { get; }
    }
}
