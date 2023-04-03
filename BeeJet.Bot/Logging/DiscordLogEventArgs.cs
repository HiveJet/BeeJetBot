namespace BeeJet.Bot.Logging
{
    public class DiscordLogEventArgs
    {
        public string Message { get; }

        public DiscordLogEventArgs(string message) 
        { 
            Message = message; 
        }
    }
}
