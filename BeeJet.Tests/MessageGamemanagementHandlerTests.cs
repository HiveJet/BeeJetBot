using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Interfaces;
using Discord;
using Discord.Commands;
using NSubstitute;

namespace BeeJet.Tests
{
    public class MessageGamemanagementHandlerTests
    {
        [Test]
        public async Task AddGameAsync_WithExistingChannel_SendsAlreadyAddedMessage()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var messageSubstitute = Substitute.For<IUserMessage>();
            messageSubstitute.Author.Returns(userSubstitute);

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(true);
            guildManagerSubstitute.ChannelExistsAsync(Arg.Any<string>()).Returns(true);

            var gameHandler = new MessageGameManagementHandler(messageSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await gameHandler.MessageChannel.Received().SendMessageAsync("This game already has a channel");
            await gameHandler.GuildManager.DidNotReceive().CreateGameChannelAsync("TestChannel");
        }

        [Test]
        public async Task AddGameAsync_WithUserWithoutAdminRole_SendsBeeJetBotAdminRequiredMessage()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var messageSubstitute = Substitute.For<IUserMessage>();
            messageSubstitute.Author.Returns(userSubstitute);

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(false);

            var gameHandler = new MessageGameManagementHandler(messageSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await gameHandler.MessageChannel.Received().SendMessageAsync("You don't have permission to create game channels");
            await gameHandler.GuildManager.DidNotReceive().ChannelExistsAsync("TestChannel");
        }

        [Test]
        public async Task AddGameAsync_WithErrorCreatingGameChannel_SendsUnableToCreateChannelMessage()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var messageSubstitute = Substitute.For<IUserMessage>();
            messageSubstitute.Author.Returns(userSubstitute);

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(true);
            guildManagerSubstitute.ChannelExistsAsync(Arg.Any<string>()).Returns(false);
            guildManagerSubstitute.CreateGameChannelAsync(Arg.Any<string>()).Returns(default(ITextChannel));

            var gameHandler = new MessageGameManagementHandler(messageSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await gameHandler.MessageChannel.Received().SendMessageAsync("Unable to create game channel for TestChannel");
            await gameHandler.MessageChannel.DidNotReceive().SendMessageAsync("This is the channel for TestChannel");
        }

        [Test]
        public async Task AddGameAsync_WithoutGameChannelAndWithAdminRole_SendsGameChannelCreatedMessageToCreatedGameChannel()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var messageSubstitute = Substitute.For<IUserMessage>();
            messageSubstitute.Author.Returns(userSubstitute);

            var channelSubsitute = Substitute.For<ITextChannel>();
            var gameListChannelSubstitute = Substitute.For<ITextChannel>();

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(true);
            guildManagerSubstitute.ChannelExistsAsync(Arg.Any<string>()).Returns(false);
            guildManagerSubstitute.CreateGameChannelAsync(Arg.Any<string>()).Returns(channelSubsitute);
            guildManagerSubstitute.GetMainGameListChannelAsync().Returns(gameListChannelSubstitute);

            var gameHandler = new MessageGameManagementHandler(messageSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await channelSubsitute.Received().SendMessageAsync("This is the channel for TestChannel");
            await gameListChannelSubstitute.Received().SendMessageAsync("Click to join channel for TestChannel", components: Arg.Any<MessageComponent>());
        }

        [Test]
        public async Task JoinGamePressed_WithInvalidGameMessage_ReturnsFalse()
        {
            var userSubstitute = Substitute.For<IGuildUser>();

            var messageSubstitute = Substitute.For<IUserMessage>();
            messageSubstitute.Author.Returns(userSubstitute);
            messageSubstitute.Content.Returns("Invalid@$#GameName");

            var guildManagerSubstitute = Substitute.For<IGuildManager>();

            var gameHandler = new MessageGameManagementHandler(messageSubstitute, guildManagerSubstitute);
            await gameHandler.JoinGamePressed();

            await gameHandler.MessageChannel.Received().SendMessageAsync("Invalid game name");
        }

    }
}
