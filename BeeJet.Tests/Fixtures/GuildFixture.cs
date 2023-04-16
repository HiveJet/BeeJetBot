using BeeJet.Bot;
using Discord;
using NSubstitute;

namespace BeeJet.Tests.Fixtures
{
    internal static class GuildFixture
    {
        public static IGuild GuildWithAdminRole
        {
            get
            {
                var role = RoleFixture.AdminRole;
                var guild = Substitute.For<IGuild>();
                guild.Roles.Returns(new IRole[] { role });
                return guild;
            }
        }
    }
}
