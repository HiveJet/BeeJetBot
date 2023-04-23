using BeeJet.Bot.Commands;
using Discord;

namespace BeeJet.Tests.Proxy
{
    public class SlashCommandContextProxy : SlashCommandContext
    {
        public SlashCommandContextProxy(ISlashCommandInteraction context, IDiscordClient client, IUser user, IGuild guild) : base(context, client)
        {
            User = user;
            Guild = guild;
        }
    }
}
