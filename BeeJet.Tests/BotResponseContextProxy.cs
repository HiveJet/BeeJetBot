using BeeJet.Bot.Commands;
using Discord;

namespace BeeJet.Tests
{
    public class BotResponseContextProxy : SlashCommandContext
    {
        public BotResponseContextProxy(ISlashCommandInteraction context, IDiscordClient client, IUser user, IGuild guild) : base(context, client)
        {
            User = user;
            Guild = guild;
        }
    }
}
