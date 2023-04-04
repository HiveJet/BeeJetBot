using BeeJet.Bot.Commands.Sources;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeeJet.Bot.Commands.Handlers.GameManagement;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    internal class SteamButtonsPressedHandler
    {

        [ButtonPressedHandler("join-game-", startsWith: true)]
        internal static async Task JoinGamePressed(SocketMessageComponent component)
        {
            var match = new Regex("join-game-([a-zA-Z0-9\\s]*)-------([a-zA-Z0-9-\\s]*)",  RegexOptions.Compiled).Match(component.Data.CustomId);
            if (match.Success)
            {
                var gameName = match.Groups[1].Value;
                var category = match.Groups[2].Value;
                var channel = GetGameChannel(component.Message, gameName, category);
                await GameButtonsPressedHandler.GivePermissionToJoinChannel(component.User, channel);
                await component.DeferAsync(true);
            }
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName, string categoryName)
        {
            var textChannels = ((SocketTextChannel)message.Channel).Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && b.Category.Name == categoryName.Replace("-", " "));
            return gameChannel;
        }
    }
}
