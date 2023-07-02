using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Storage.Interfaces;
using BeeJet.Tests.Fixtures;
using BeeJet.Tests.Proxy;
using Discord;
using NSubstitute;
using FluentAssertions;
using BeeJet.Bot.Interfaces;

namespace BeeJet.Tests
{
    public class GameButtonsPressedHandlerTests
    {
        [Test]
        public async Task JoinGamePressed_ShouldNotJoinChannel_WhenCalledWithoutGameName()
        {
            var user = UserFixture.UserWithAdminRole;
            var guild = GuildFixture.GuildWithAdminRole;
            var commandInteraction = Substitute.For<IComponentInteraction>();
            var client = Substitute.For<IDiscordClient>();
            var message = Substitute.For<IUserMessage>();
            message.Content.Returns(string.Empty);
            var context = Substitute.For<ButtonPressedContextProxy>(commandInteraction, client, user, guild, message);
            var buttonContext = Substitute.For<IButtonContextDb>();
            buttonContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(default(IButtonContext));
            var pressedHandler = new GameButtonsPressedHandler(buttonContext);
            pressedHandler.Context = context;
            await pressedHandler.JoinGamePressed();

            await guild.DidNotReceive().GetChannelsAsync();
            await context.ComponentInteraction.Received().DeferAsync();
        }

        [Test]
        public async Task JoinGamePressed_ShouldNotJoinChannel_WhenPushedWithoutGameChannel()
        {
            var buttonDbContext = Substitute.For<IButtonContextDb, IResponseContext>();
            buttonDbContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(default(IButtonContext));
            var buttonPressedContext = Substitute.For<IButtonPressedContext>();

            var pressedHandler = new GameButtonsPressedHandler(buttonDbContext);
            pressedHandler.Context = buttonPressedContext;

            await pressedHandler.JoinGamePressed();

            await pressedHandler.Context.Client.DidNotReceive().GetChannelAsync(Arg.Any<ulong>());
        }

        [Test]
        public async Task JoinGamePressed_ShouldAddUserToChannel_WhenPushedWithExistingGameChannelAndNoPermissions()
        {
            var channelId = (ulong)1;

            var user = Substitute.For<IGuildUser>();
            user.Id.Returns((ulong)2);

            var channel = Substitute.For<ITextChannel>();
            channel.Id.Returns(channelId);

            var buttonContext = Substitute.For<IButtonContext>();
            buttonContext.HandlerContext.Returns(channelId.ToString());

            var buttonDbContext = Substitute.For<IButtonContextDb, IResponseContext>();
            buttonDbContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(buttonContext);

            var buttonPressedContext = Substitute.For<IButtonPressedContext>();
            buttonPressedContext.Client.GetChannelAsync(Arg.Any<ulong>()).Returns(channel);
            buttonPressedContext.User.Returns(user);

            var pressedHandler = new GameButtonsPressedHandler(buttonDbContext);
            pressedHandler.Context = buttonPressedContext;

            await pressedHandler.JoinGamePressed();

            await channel.Received().AddPermissionOverwriteAsync(user, Arg.Any<OverwritePermissions>());
            await channel.Received().SendMessageAsync($"Welcome <@{user.Id}>");
        }

        [Test]
        public async Task JoinGamePressed_ShouldDoNothing_WhenUserIsAlreadyAddedToChannel()
        {
            var channelId = (ulong)1;

            var user = Substitute.For<IGuildUser>();
            user.Id.Returns((ulong)2);
            var guildUserList = new List<IReadOnlyCollection<IGuildUser>>() { new List<IGuildUser>() { user }.AsReadOnly() };
            var users = AsyncEnumerable.ToAsyncEnumerable(guildUserList);

            var channel = Substitute.For<ITextChannel>();
            channel.Id.Returns(channelId);
            channel.GetUsersAsync().Returns(users);

            var buttonContext = Substitute.For<IButtonContext>();
            buttonContext.HandlerContext.Returns(channelId.ToString());

            var buttonDbContext = Substitute.For<IButtonContextDb, IResponseContext>();
            buttonDbContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(buttonContext);

            var buttonPressedContext = Substitute.For<IButtonPressedContext>();
            buttonPressedContext.Client.GetChannelAsync(Arg.Any<ulong>()).Returns(channel);
            buttonPressedContext.User.Returns(user);

            var pressedHandler = new GameButtonsPressedHandler(buttonDbContext);
            pressedHandler.Context = buttonPressedContext;

            await pressedHandler.JoinGamePressed();

            await channel.DidNotReceive().AddPermissionOverwriteAsync(user, Arg.Any<OverwritePermissions>());
            await channel.DidNotReceive().SendMessageAsync($"Welcome <@{user.Id}>");
        }

        [Test]
        public async Task LeaveGamePressed_ShouldNotLeaveChannel_WhenPushedWithoutGameChannel()
        {
            var buttonDbContext = Substitute.For<IButtonContextDb, IResponseContext>();
            buttonDbContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(default(IButtonContext));
            var buttonPressedContext = Substitute.For<IButtonPressedContext>();

            var pressedHandler = new GameButtonsPressedHandler(buttonDbContext);
            pressedHandler.Context = buttonPressedContext;

            await pressedHandler.LeaveGamePressed();

            await pressedHandler.Context.Client.DidNotReceive().GetChannelAsync(Arg.Any<ulong>());
        }

        [Test]
        public async Task LeaveGamePressed_ShouldRemoveUserFromChannel_WhenPushedWithExistingGameChannel()
        {
            var channelId = (ulong)1;

            var user = Substitute.For<IGuildUser>();
            user.Id.Returns((ulong)2);

            var channel = Substitute.For<ITextChannel>();
            channel.Id.Returns(channelId);

            var buttonContext = Substitute.For<IButtonContext>();
            buttonContext.HandlerContext.Returns(channelId.ToString());

            var buttonDbContext = Substitute.For<IButtonContextDb, IResponseContext>();
            buttonDbContext.GetButtonContext(Arg.Any<ulong>(), Arg.Any<string>()).Returns(buttonContext);

            var buttonPressedContext = Substitute.For<IButtonPressedContext>();
            buttonPressedContext.Client.GetChannelAsync(Arg.Any<ulong>()).Returns(channel);
            buttonPressedContext.User.Returns(user);

            var pressedHandler = new GameButtonsPressedHandler(buttonDbContext);
            pressedHandler.Context = buttonPressedContext;

            await pressedHandler.LeaveGamePressed();

            await channel.Received().AddPermissionOverwriteAsync(user, Arg.Any<OverwritePermissions>());
            await channel.Received().SendMessageAsync($"<@{user.Id}> has left the channel");
        }
    }
}
