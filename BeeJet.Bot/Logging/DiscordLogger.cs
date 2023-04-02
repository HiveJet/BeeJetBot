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
        public Func<object, DiscordLogEventArgs, Task> LoggedEvent;

        public Task Log(LogMessage message)
        {
            return Log($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}", true);
        }

        public Task Log(string message)
        {
            return Log(message, true);
        }

        public Task LogWithoutBroadcast(LogMessage message)
        {
            return Log($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}", false);
        }

        private Task Log(string message, bool broadCast)
        {
            Console.WriteLine(message);

            if (broadCast)
            {
                LoggedEvent?.Invoke(this, new DiscordLogEventArgs(message));
            }
            return Task.CompletedTask;
        }
    }
}
