using BeeJet.Bot;
using Discord;
using NSubstitute;

namespace BeeJet.Tests.Fixtures
{
    internal static class UserFixture
    {
        public static IUser UserWithAdminRole
        {
            get
            { 
                var user = Substitute.For<IGuildUser>();
                user.RoleIds.Returns(new ulong[] { 1 });
                return user;
            }
        }
    }
}
