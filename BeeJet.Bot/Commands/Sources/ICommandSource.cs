using BeeJet.Bot.Commands.Handlers;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Sources
{
    internal interface ICommandSource
    {
        Task RegisterCommands(IGuild guild);
        string[] GetCommandNames();
        SlashCommandExecutedHandler GetCommandHandler(string commandName, SocketSlashCommand socketSlashCommand);
    }
}