using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace BeeJet.Bot.Logging
{
    public class DiscordLogger
    {
        public Func<object, DiscordLogEventArgs, Task> BroadcastLogEvent;

        public Task Log(LogMessage message)
        {
            return Log(message, true);
        }

        public Task Log(string message)
        {
            return Log(message, true);
        }

        public Task LogWithoutBroadcast(LogMessage message)
        {
            return Log(message, false);
        }

        public Task Log(LogMessage message, bool broadCast)
        {
            return Log($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}", broadCast);
        }

        public Task Log(string message, bool broadCast)
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
