using BeeJet.Bot;
using Discord;
using NSubstitute;

namespace BeeJet.Tests.Fixtures
{
    internal static class RoleFixture
    {
        public static IRole AdminRole
        {
            get
            {
                var role = Substitute.For<IRole>();
                role.Id.Returns((ulong)1);
                role.Name.Returns(BeeJetBot.BOT_ADMIN_ROLE_NAME);
                return role;
            }
        }
    }
}
