using BeeJet.Bot.ClientHandlers;
using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Commands.Handlers.GameManagement;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace BeeJet.Bot.Commands.Sources
{
    internal class GameManagementCommandSource : ICommandSource
    {
        internal static readonly string ChannelName = "Game-channels";
        internal const string JointButtonId = "join-game-id";
        internal const string LeaveButtonId = "leave-game-id";
        internal const string AddGameCommand = "add-game";

        public GameManagementCommandSource()
        {

        }

        public static async Task JoinGamePressed(IUserMessage message, SocketUser user)
        {
            if (TryGetGameName(message, out string gameName))
            {
                await JoinGameAsync(message, user, gameName);
            }
        }

        private static async Task JoinGameAsync(IUserMessage message, SocketUser user, string gameName)
        {
            SocketTextChannel gameChannel = GetGameChannel(message, gameName);
            if (gameChannel != null)
            {
                await GivePremissionToJoinChannel(user, gameChannel);
            }
        }

        public static async Task GivePremissionToJoinChannel(SocketUser user, SocketTextChannel gameChannel)
        {
            if (!gameChannel.Users.Any(b => b.Id == user.Id))
            {
                var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
                await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                await gameChannel.SendMessageAsync($"Welcome <@{user.Id}>");
            }
        }

        public static async Task LeaveGamePressed(IUserMessage message, SocketUser user)
        {
            if (TryGetGameName(message, out string gameName))
            {
                ITextChannel gameChannel = GetGameChannel(message, gameName);
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
                    await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                    await gameChannel.SendMessageAsync($"<@{user.Id}> has left the channel");
                }
            }
        }

        private static bool TryGetGameName(IUserMessage message, out string gameName)
        {
            var regex = new Regex(@"Click to join channel for ([a-zA-Z0-9\s]*)");

            var matching = regex.Match(message.Content);
            if (matching.Success)
            {
                gameName = matching.Groups[1].Value.Trim().Replace(" ", "-");
                return true;
            }
            gameName = null;
            return false;
        }

        private static SocketTextChannel GetGameChannel(IUserMessage message, string gameName)
        {
            var socketMessageChannel = message.Channel as SocketTextChannel;
            var textChannels = socketMessageChannel.Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && b.CategoryId == socketMessageChannel.CategoryId);
            return gameChannel;
        }

        public async Task RegisterCommands(IGuild guild)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("add-game");
            guildCommand.WithDescription("Add game channel");
            guildCommand.AddOption("game", ApplicationCommandOptionType.String, "The name of the game", isRequired: true);
            guildCommand.AddOption("category", ApplicationCommandOptionType.String, "Add to category", isRequired: false);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }

        public string[] GetCommandNames()
        {
            return new string[] { AddGameCommand };
        }

        public SlashCommandExecutedHandler GetCommandHandler(string commandName, SocketSlashCommand socketSlashCommand)
        {
            switch (commandName)
            {
                case AddGameCommand:
                    return new AddGameCommandHandler(socketSlashCommand);
            }
            return null;
        }

    }
}
