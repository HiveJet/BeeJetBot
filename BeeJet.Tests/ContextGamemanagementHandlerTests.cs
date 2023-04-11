using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Interfaces;
using Discord;
using Discord.Commands;
using NSubstitute;

namespace BeeJet.Tests
{
    public class ContextGamemanagementHandlerTests
    {
        [Test]
        public async Task AddGameAsync_WithExistingChannel_SendsAlreadyAddedMessage()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var contextSubstitute = Substitute.For<ICommandContext>();
            contextSubstitute.User.Returns(userSubstitute);

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(true);
            guildManagerSubstitute.ChannelExistsAsync(Arg.Any<string>()).Returns(true);

            var gameHandler = new ContextGameManagementHandler(contextSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await gameHandler.MessageChannel.Received().SendMessageAsync("This game already has a channel");
        }

        [Test]
        public async Task AddGameAsync_WithUserWithoutAdminRole_SendsBeeJetBotAdminRequiredMessage()
        {
            var userSubstitute = Substitute.For<IGuildUser>();
            var contextSubstitute = Substitute.For<ICommandContext>();
            contextSubstitute.User.Returns(userSubstitute);

            var guildManagerSubstitute = Substitute.For<IGuildManager>();
            guildManagerSubstitute.IsAdmin(Arg.Any<IGuildUser>()).Returns(false);

            var gameHandler = new ContextGameManagementHandler(contextSubstitute, guildManagerSubstitute);
            await gameHandler.AddGameAsync("TestChannel");
            await gameHandler.MessageChannel.Received().SendMessageAsync("You don't have permission to create game channels");
        }
    }
}
