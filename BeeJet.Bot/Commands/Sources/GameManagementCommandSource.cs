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
