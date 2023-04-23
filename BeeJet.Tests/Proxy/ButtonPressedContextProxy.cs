using BeeJet.Bot.Commands;
using Discord;

namespace BeeJet.Tests.Proxy
{
    public class ButtonPressedContextProxy : ButtonPressedContext
    {
        public ButtonPressedContextProxy(IComponentInteraction context, IDiscordClient client, IUser user, IGuild guild, IUserMessage message) : base(context, client)
        {
            User = user;
            Guild = guild;
            Message = message;
        }

        public void SetChannel(IChannel channel)
        {
            Channel = channel;
        }
    }
}
