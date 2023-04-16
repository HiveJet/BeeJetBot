using BeeJet.Bot.Extensions;
using Discord;

namespace BeeJet.Bot.Commands
{
    public class ButtonPressedContext : IBotResponseContext
    {
        public IComponentInteraction ComponentInteraction { get; }

        public IGuild Guild { get; set; }

        public IChannel Channel { get; set; }

        public IUserMessage Message { get; }

        public IUser User { get; set; }
        public IDiscordClient Client { get; private set; }

        public ButtonPressedContext()
        {
        }

        public ButtonPressedContext(IComponentInteraction context, IDiscordClient client)
        {
            ComponentInteraction = context;
            User = ComponentInteraction.User;
            Message = context.Message;
            Client = client;
        }

        public virtual async Task Initialize()
        {
            if (ComponentInteraction.GuildId.HasValue)
            {
                Guild = await Client.GetGuildAsync(ComponentInteraction.GuildId.Value);
            }
            if (ComponentInteraction.ChannelId.HasValue)
            {
                Channel = await Client.GetChannelAsync(ComponentInteraction.ChannelId.Value);
            }
            await Guild.AddAdminRoleIfNeeded();
        }
    }
}
