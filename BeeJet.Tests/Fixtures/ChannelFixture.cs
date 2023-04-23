using Discord;
using NSubstitute;

namespace BeeJet.Tests.Fixtures
{
    internal class ChannelFixture
    {
        public static ICategoryChannel DefaultCategoryChannel
        {
            get
            {
                var categoryChannel = Substitute.For<ICategoryChannel, IEntity<ulong>>();
                categoryChannel.Id.Returns((ulong)1);
                return categoryChannel;
            }
        }

        public static ITextChannel TextChannelWithDefaultCategory
        {
            get
            {
                var textChannel = Substitute.For<ITextChannel, IEntity<ulong>>();
                textChannel.CategoryId.Returns((ulong)1);
                textChannel.Name.Returns("TestGame");
                return textChannel;
            }
        }
    }
}
