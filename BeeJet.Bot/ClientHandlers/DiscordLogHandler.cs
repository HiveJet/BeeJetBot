using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeJet.Bot.Commands.Sources;
using BeeJet.Bot.Extensions;
using BeeJet.Bot.Logging;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    internal class DiscordLogHandler
    {
        private readonly DiscordSocketClient _client;

        private const string LOGGING_CHANNEL_CATEGORY = "mod";
        private const string LOGGING_CHANNEL_NAME = "beejet-logs";

        public DiscordLogHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        internal async Task OnLoggedMessage(object sender, DiscordLogEventArgs e)
        {
            // Here we can push the logged message to a #bot-log channel instead of Console.WriteLine
            foreach (var guild in _client.GetRelevantGuilds())
            {
                var channelCategory = await guild.GetOrCreateCategory(LOGGING_CHANNEL_CATEGORY);
                var logChannel = await GetOrCreateLogChannel(guild, channelCategory);
                
                if (logChannel != null)
                {
                    _ = await logChannel.SendMessageAsync($"LOG: {e.Message}");
                }
                else
                {
                    Console.WriteLine($"{nameof(DiscordLogHandler)}: Cannot find logging channel");
                }
            }
        }

        // TODO: This contains sort of duplicate code; perhaps also add a GetOrCreateChannel extension method on Guild. Channel and Permission stuff should be a parameter
        private static async Task<ITextChannel> GetOrCreateLogChannel(SocketGuild guild, ICategoryChannel logChannelCategory)
        {
            if (logChannelCategory == null || logChannelCategory is not SocketCategoryChannel socketLogChannelCategory)
            {
                return null;
            }

            // Get logging channel from found category
            var logChannels = socketLogChannelCategory.Channels
                .Where(b => b.Name.Equals(LOGGING_CHANNEL_NAME, StringComparison.OrdinalIgnoreCase));

            // If no logging channel found, create one
            if (!logChannels.Any())
            {
                var logChannel = await guild.CreateTextChannelAsync(LOGGING_CHANNEL_NAME, (properties) => properties.CategoryId = logChannelCategory.Id);
                var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
                await logChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissionOverrides);
                return logChannel;
            }
            else
            {
                var textChannels = socketLogChannelCategory.Channels.OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(LOGGING_CHANNEL_NAME, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
