using Discord.Commands;

namespace BeeJet.Bot.Commands.Handlers
{
    public class BaseHandler
    {
        public SocketCommandContext Context { get; set; }

        public BaseHandler(SocketCommandContext context)
        {
            Context = context;
        }

        protected async Task AddAdminRoleIfNeeded()
        {
            if (!Context.Guild.Roles.Any(b => b.Name == "BeeJetBotAdmin"))
            {
                await Context.Guild.CreateRoleAsync("BeeJetBotAdmin");
            }
        }

        protected ulong GetAdminRoleId()
        {
            return Context.Guild.Roles.FirstOrDefault(b => b.Name == "BeeJetBotAdmin").Id;
        }
    }
}