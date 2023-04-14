using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using BeeJet.Bot.Commands.Handlers.GameManagement;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    public class SteamButtonsPressedHandler : ButtonPressedHandler
    {

        [ButtonPressedHandler("join-game-id-", startsWith: true)]
        public async Task JoinGamePressed()
        {
            var match = new Regex("join-game-id-([a-zA-Z0-9\\s]*)-------([a-zA-Z0-9-\\s]*)",  RegexOptions.Compiled).Match(Context.ComponentInteraction.Data.CustomId);
            if (match.Success)
            {
                var gameName = match.Groups[1].Value;
                var category = match.Groups[2].Value;
                var channel = GetGameChannel(Context.Message, gameName, category);
                await GameButtonsPressedHandler.GivePermissionToJoinChannel(Context.User, channel);
            }
            await Context.ComponentInteraction.DeferAsync(true);
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName, string categoryName)
        {
            var textChannels = ((SocketTextChannel)message.Channel).Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && b.Category.Name == categoryName.Replace("-", " "));
            return gameChannel;
        }
    }
}
