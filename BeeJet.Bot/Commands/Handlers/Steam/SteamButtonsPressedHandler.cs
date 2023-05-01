using Discord.WebSocket;
using Discord;
using System.Text.RegularExpressions;
using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Storage.Interfaces;

namespace BeeJet.Bot.Commands.Handlers.Steam
{
    public class SteamButtonsPressedHandler : ButtonPressedHandler
    {

        private IButtonContextDb _buttonContextDb;

        public SteamButtonsPressedHandler(IButtonContextDb buttonContextDb)
        {
            _buttonContextDb = buttonContextDb;
        }

        [ButtonPressedHandler("join-game-id-", startsWith: true)]
        public async Task JoinGamePressed()
        {
            if (GetChannelId(out ulong channelId))
            {
                var channel = await Context.Client.GetChannelAsync(channelId);
                if (channel is ITextChannel textChannel)
                {
                    await GameButtonsPressedHandler.GivePermissionToJoinChannel(Context.User, textChannel);
                }
            }
            await Context.ComponentInteraction.DeferAsync(true);
        }

        private bool GetChannelId(out ulong channelId)
        {
            var context = _buttonContextDb.GetButtonContextForMessageIdAndCustomId(Context.Message.Id, Context.ComponentInteraction.Data.CustomId);
            if (context == null)
            {
                channelId = 0;
                return false;
            }
            else
            {
                channelId = ulong.Parse((string)context.HandlerContext);//LiteDb doesn't support ulong
                return true;
            }
        }
     
    }
}
