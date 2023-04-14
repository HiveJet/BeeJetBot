using Discord;

namespace BeeJet.Bot.Logging
{
    public class DiscordLogger
    {
        public Func<object, DiscordLogEventArgs, Task> BroadcastLogEvent;

        public Task LogAndBroadCast(LogMessage message)
        {
            return Log(message, true);
        }

        public Task LogAndBroadCast(string message)
        {
            return Log(message, true);
        }

        public Task Log(LogMessage message)
        {
            return Log(message, false);
        }

        public Task Log(string message)
        {
            return Log(message, false);
        }

        private Task Log(LogMessage message, bool broadCast)
        {
            return Log($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}", broadCast);
        }

        private Task Log(string message, bool broadCast)
        {
            Console.WriteLine(message);

            if (broadCast)
            {
                BroadcastLogEvent?.Invoke(this, new DiscordLogEventArgs(message));
            }
            return Task.CompletedTask;
        }
    }
}
