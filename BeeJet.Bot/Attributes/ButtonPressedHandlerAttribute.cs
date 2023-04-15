namespace BeeJet.Bot
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonPressedHandlerAttribute : Attribute
    {
        public ButtonPressedHandlerAttribute(string customId, bool startsWith = false)
        {
            CustomId = customId;
            StartsWith = startsWith;
        }

        public string CustomId { get; }
        public bool StartsWith { get; }
    }
}
