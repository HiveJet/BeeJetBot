using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Tests.Fixtures;
using BeeJet.Tests.Proxy;
using Discord;
using NSubstitute;

namespace BeeJet.Tests
{
    public class GameButtonsPressedHandlerTests
    {
        [Test]
        public async Task JoinGamePressed_WithoutGameName_DoesntJoinChannel()
        {
            var user = UserFixture.UserWithAdminRole;
            var guild = GuildFixture.GuildWithAdminRole;
            var commandInteraction = Substitute.For<IComponentInteraction>();
            var client = Substitute.For<IDiscordClient>();
            var message = Substitute.For<IUserMessage>();
            message.Content.Returns(string.Empty);
            var context = Substitute.For<ButtonPressedContextProxy>(commandInteraction, client, user, guild, message);
            var pressedHandler = new GameButtonsPressedHandler();
            pressedHandler.Context = context;
            await pressedHandler.JoinGamePressed();

            await guild.DidNotReceive().GetChannelsAsync();
            await context.ComponentInteraction.Received().DeferAsync();
        }

        [Test]
        public async Task JoinGamePressed_WitGameNameWithoutGameChannel_DoesntJoinChannel()
        {
            var user = UserFixture.UserWithAdminRole;
            user.Id.Returns((ulong)1);
            var guild = GuildFixture.GuildWithAdminRole;
            var channel = Substitute.For<INestedChannel>();
            var commandInteraction = Substitute.For<IComponentInteraction>();
            var client = Substitute.For<IDiscordClient>();

            var joinMessage = $"Click to join channel for Test";
            var message = Substitute.For<IUserMessage, IMessage>();
            message.Content.Returns(joinMessage);

            var context = Substitute.For<ButtonPressedContextProxy>(commandInteraction, client, user, guild, message);
            var pressedHandler = new GameButtonsPressedHandler();
            pressedHandler.Context = context;
            context.SetChannel(channel);
            await pressedHandler.JoinGamePressed();

            await guild.DidNotReceive().GetChannelsAsync();
            await context.ComponentInteraction.Received().DeferAsync();
        }

        [Test]
        public async Task JoinGamePressed_WitGameNameWithGameChannel_JoinsChannel()
        {
            var user = UserFixture.UserWithAdminRole;
            user.Id.Returns((ulong)1);
            var categoryChannel = ChannelFixture.DefaultCategoryChannel;
            var channel = ChannelFixture.TextChannelWithDefaultCategory;            
            var guild = GuildFixture.GuildWithAdminRole;
            guild.GetChannelsAsync().Returns(new IGuildChannel[] { categoryChannel, channel });

            var commandInteraction = Substitute.For<IComponentInteraction>();
            commandInteraction.ChannelId.Returns(channel.Id);
            var client = Substitute.For<IDiscordClient>();

            var joinMessage = $"Click to join channel for {channel.Name}";
            var message = Substitute.For<IUserMessage, IMessage>();
            message.Content.Returns(joinMessage);
            
            var context = Substitute.For<ButtonPressedContextProxy>(commandInteraction, client, user, guild, message);
            context.SetChannel(channel);
            var pressedHandler = new GameButtonsPressedHandler();
            pressedHandler.Context = context;
            await pressedHandler.JoinGamePressed();

            var expectedMessage = "Welcome <@1>";
            await channel.Received().SendMessageAsync(expectedMessage);
            await context.ComponentInteraction.Received().DeferAsync();
        }
    }
}
