using BeeJet.Bot;
using BeeJet.Bot.Commands;
using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Bot.Services;
using BeeJet.Tests.Fixtures;
using Discord;
using NSubstitute;

namespace BeeJet.Tests
{
    public class AddGameCommandHandlerTests
    {
        [Test]
        public async Task AddGameAsync_WithoutAdminUser_SendsAddAdminRoleMessage()
        {
            var user = Substitute.For<IGuildUser>();
            var guild = GuildFixture.GuildWithAdminRole;

            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);

            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();

            var context = Substitute.For<BotResponseContextProxy>(commandInteraction, client, user, guild);

            await commandHandler.AddGameAsync("TestGame", "Test", context);
            await context.SlashCommandInteraction.Received().RespondAsync($"To add a game you need the role '{BeeJetBot.BOT_ADMIN_ROLE_NAME}'", ephemeral: true);
            await context.Guild.DidNotReceiveWithAnyArgs().CreateTextChannelAsync(Arg.Any<string>(), func: Arg.Any<Action<TextChannelProperties>>());
        }

        [Test]
        public async Task AddGameAsync_WithExistingGameChannel_SendsGameAlreadyHasChannelMessage()
        {
            var role = Substitute.For<IRole>();
            role.Id.Returns((ulong)1);
            role.Name.Returns(BeeJetBot.BOT_ADMIN_ROLE_NAME);

            var user = Substitute.For<IGuildUser>();
            user.RoleIds.Returns(new ulong[] { (ulong)1 });

            var categoryChannel = Substitute.For<ICategoryChannel>();
            categoryChannel.Name.Returns("Test");
            categoryChannel.Id.Returns((ulong)1);
            var gameChannel = Substitute.For<INestedChannel>();
            gameChannel.Name.Returns("TestGame");
            gameChannel.CategoryId.Returns((ulong)1);

            var guild = Substitute.For<IGuild>();
            guild.Roles.Returns(new IRole[] { role });
            guild.GetChannelsAsync().Returns(new IGuildChannel[] { gameChannel, categoryChannel });
            categoryChannel.Guild.Returns(guild);

            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();
          
            var context = Substitute.For<BotResponseContextProxy>(commandInteraction, client, user, guild);

            //I need a SocketCategoryChannel which I can't substitute or create
            await commandHandler.AddGameAsync("TestGame", "Test", context);
            await context.SlashCommandInteraction.Received().RespondAsync($"This game already has a channel", ephemeral: true);
        }

        [Test]
        public async Task AddGameAsync_WithoutChannel_CreatesGameChannel()
        {
            var gameName = "TestGame";
            var channel = Substitute.For<ITextChannel>();
            channel.Name.Returns(gameName);

            var guild = GuildFixture.GuildWithAdminRole;
            guild.CreateTextChannelAsync(Arg.Any<string>(), func: Arg.Any<Action<TextChannelProperties>>()).Returns(channel);
            var user = UserFixture.UserWithAdminRole;

            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();

            var context = Substitute.For<BotResponseContextProxy>(commandInteraction, client, user,guild);

            await commandHandler.AddGameAsync(gameName, "Test", context);
            await context.SlashCommandInteraction.Received().RespondAsync("Channel created", ephemeral: true);
            await channel.Received().SendMessageAsync($"This is the channel for {gameName}", embed: Arg.Any<Embed>());
        }

        [Test]
        public void RegisterOptions_WithBuilder_CreatesGameAndCategoryOption()
        {
            var gameOptionName = "game";
            var categoryOptionName = "category";
            var stringType = ApplicationCommandOptionType.String;

            var builder = new SlashCommandBuilder();
            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            commandHandler.RegisterOptions(builder);
            var properties = builder.Build();

            Assert.That(gameOptionName, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[0].Name));
            Assert.That(stringType, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[0].Type));
            Assert.IsTrue(((List<ApplicationCommandOptionProperties>)properties.Options)[0].IsRequired);

            Assert.That(categoryOptionName, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[1].Name));
            Assert.That(stringType, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[1].Type));
            Assert.IsFalse(((List<ApplicationCommandOptionProperties>)properties.Options)[1].IsRequired);
        }
      
    }


}
