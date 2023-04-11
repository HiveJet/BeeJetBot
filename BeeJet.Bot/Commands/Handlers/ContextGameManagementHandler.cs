using BeeJet.Bot.Interfaces;
using BeeJet.Bot.Managers;
using Discord;
using Discord.Commands;

namespace BeeJet.Bot.Commands.Handlers
{
    public class ContextGameManagementHandler : BaseGameManagementHandler
    {
        private ICommandContext _context;

        public ContextGameManagementHandler(ICommandContext context, IGuildManager guildManager)
            :base(guildManager)
        {
            _context = context;
        }

        public override IMessageChannel MessageChannel => _context.Channel;

        public override IGuildUser User => (IGuildUser)_context.User;
    }
}
