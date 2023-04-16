using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Commands.Handlers
{
    public class ContextGameManagementHandler : BaseGameManagementHandler
    {
        private ICommandContext _context;

        public ContextGameManagementHandler(ICommandContext context, IGuildManager guildManager)
            :base(guildManager, context.User)
        {
            _context = context;
        }

        public override IMessageChannel MessageChannel => _context.Channel;

    }
}
