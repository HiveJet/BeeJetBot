using BeeJet.Bot.Extensions;
using System.Text.RegularExpressions;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    public class SteamButtonsPressedHandler : ButtonPressedHandler
    {

        [ButtonPressedHandler("join-game-id-", startsWith: true)]
        public async Task JoinGamePressed()
        {
            if (TryParseButtonCustomId(out string gameName, out string category))
            {
                var channel = await Context.Guild.GetTextChannelAsync(gameName, category);
                await Context.User.GivePermissionToJoinChannel(channel, $"Welcome <@{Context.User.Id}>");
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
    }
}
