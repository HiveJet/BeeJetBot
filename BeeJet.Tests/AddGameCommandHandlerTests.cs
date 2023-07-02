using BeeJet.Bot;
using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Bot.Services;
using BeeJet.Tests.Fixtures;
using BeeJet.Tests.Proxy;
using Discord;
using NSubstitute;

namespace BeeJet.Tests
{
    public class AddGameCommandHandlerTests
    {
        [Test]
        public async Task AddGameAsync_ShouldSendRequiredAdminRoleMessage_WhenUserIsNotAdmin()
        {
            var user = Substitute.For<IGuildUser>();
            var guild = GuildFixture.GuildWithAdminRole;
            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();
            var context = new SlashCommandContextProxy(commandInteraction, client, user, guild);

            var commandHandler = new AddGameCommandHandler(service);
            await commandHandler.AddGameAsync("InvalidGameName", "InvalidCategory", context);

            await context.SlashCommandInteraction.Received().RespondAsync($"To add a game you need the role '{BeeJetBot.BOT_ADMIN_ROLE_NAME}'", ephemeral: true);
            await context.Guild.DidNotReceiveWithAnyArgs().CreateTextChannelAsync(Arg.Any<string>(), func: Arg.Any<Action<TextChannelProperties>>());
        }

        [Test]
        public async Task AddGameAsync_ShouldSendAlreadyHasChannelMessage_WhenGameChannelAlreadyExists()
        {
            var role = RoleFixture.AdminRole;

            var user = Substitute.For<IGuildUser>();
            user.RoleIds.Returns(new ulong[] { (ulong)1 });

            var categoryChannel = Substitute.For<ICategoryChannel>();
            categoryChannel.Name.Returns("Test");
            categoryChannel.Id.Returns((ulong)1);
            var gameChannel = Substitute.For<INestedChannel, ITextChannel>();
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
          
            var context = new SlashCommandContextProxy(commandInteraction, client, user, guild);

            await commandHandler.AddGameAsync(gameChannel.Name, categoryChannel.Name, context);
            await context.SlashCommandInteraction.Received().RespondAsync($"This game already has a channel", ephemeral: true);
        }

        [Test]
        public async Task AddGameAsync_ShouldCreateGameChannel_WhenChannelDoesntExist()
        {
            var channel = Substitute.For<ITextChannel>();
            channel.Name.Returns("TestGame");

            var guild = GuildFixture.GuildWithAdminRole;
            guild.CreateTextChannelAsync(Arg.Any<string>(), func: Arg.Any<Action<TextChannelProperties>>()).Returns(channel);
            var user = UserFixture.UserWithAdminRole;

            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();

            var context = new SlashCommandContextProxy(commandInteraction, client, user,guild);

            await commandHandler.AddGameAsync(channel.Name, "Test", context);
            await context.SlashCommandInteraction.Received().RespondAsync("Channel created", ephemeral: true);
            await channel.Received().SendMessageAsync($"This is the channel for {channel.Name}", embed: Arg.Any<Embed>());
        }

        [Test]
        public void RegisterOptions_ShouldCreateGameAndCategoryOption_WhenCalled()
        {
            var gameOptionName = "game";
            var categoryOptionName = "category";
            var stringType = ApplicationCommandOptionType.String;

            var builder = new SlashCommandBuilder();
            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            commandHandler.RegisterOptions(builder);
            var properties = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(gameOptionName, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[0].Name));
                Assert.That(stringType, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[0].Type));
                Assert.IsTrue(((List<ApplicationCommandOptionProperties>)properties.Options)[0].IsRequired);
            });

            Assert.Multiple(() =>
            {
                Assert.That(categoryOptionName, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[1].Name));
                Assert.That(stringType, Is.EqualTo(((List<ApplicationCommandOptionProperties>)properties.Options)[1].Type));
                Assert.IsFalse(((List<ApplicationCommandOptionProperties>)properties.Options)[1].IsRequired);
            });
        }

        [Test]
        public void GetCategoryName_ShouldReturnDefaultCategoryName_WhenGivenCategoryDoesntExist()
        {
            var user = UserFixture.UserWithAdminRole;
            var guild = GuildFixture.GuildWithAdminRole;

            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();
            var context = new SlashCommandContextProxy(commandInteraction, client, user, guild);

            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            commandHandler.Context = context;

            Assert.That(commandHandler.GetCategoryName(), Is.EqualTo(AddGameCommandHandler.DefaultCategoryName));
        }

        [Test]
        public void GetCategoryName_ShouldReturnCategory_WhenGivenCategoryExists()
        {
            var setCategoryName = "Test";

            var user = UserFixture.UserWithAdminRole;
            var guild = GuildFixture.GuildWithAdminRole;

            var commandInteraction = Substitute.For<ISlashCommandInteraction>();
            var client = Substitute.For<IDiscordClient>();

            var applicationOption = Substitute.For<IApplicationCommandInteractionDataOption>();
            applicationOption.Name.Returns("category");
            applicationOption.Value.Returns(setCategoryName);
            var context = new SlashCommandContextProxy(commandInteraction, client, user, guild);
            context.SlashCommandInteraction.Data.Options.Returns(new IApplicationCommandInteractionDataOption[] { applicationOption });

            var builder = new SlashCommandBuilder();
            var service = Substitute.For<IGDBService>(string.Empty, string.Empty);
            var commandHandler = new AddGameCommandHandler(service);
            commandHandler.Context = context;
            commandHandler.RegisterOptions(builder);

            Assert.That(commandHandler.GetCategoryName(), Is.EqualTo(setCategoryName));
        }
    }
}
