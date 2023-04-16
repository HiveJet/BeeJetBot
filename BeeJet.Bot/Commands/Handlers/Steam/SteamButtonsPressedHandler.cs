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
            if (TryParseButtonCustomId(out string gameName, out string category))
            {
                var channel = GetGameChannel(Context.Message, gameName, category);
                await GameButtonsPressedHandler.GivePermissionToJoinChannel(Context.User, channel);
            }
            await Context.ComponentInteraction.DeferAsync(true);
        }

        private bool TryParseButtonCustomId(out string gameName, out string category)
        {
            var match = new Regex("join-game-id-([a-zA-Z0-9\\s]*)-------([a-zA-Z0-9-\\s]*)", RegexOptions.Compiled).Match(Context.ComponentInteraction.Data.CustomId);
            if (!match.Success)
            {
                gameName = null;
                category = null;
                return false;
            }
            gameName = match.Groups[1].Value;
            category = match.Groups[2].Value;
            return true;
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName, string categoryName)
        {
            var textChannels = ((SocketTextChannel)message.Channel).Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(channel => channel.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && channel.Category.Name == categoryName.Replace("-", " "));
            return gameChannel;
        }
    }
}
