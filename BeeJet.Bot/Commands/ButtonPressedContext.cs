using BeeJet.Bot.Extensions;
using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Commands
{
    public class ButtonPressedContext : BotResponseContext, IButtonPressedContext
    {
        public IComponentInteraction ComponentInteraction { get; }

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

        public override async Task Initialize()
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
