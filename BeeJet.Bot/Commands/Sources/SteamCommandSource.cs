using BeeJet.Bot.Commands.Handlers;
using BeeJet.Bot.Commands.Handlers.GameManagement;
using BeeJet.Bot.Commands.Handlers.Steam;
using BeeJet.Bot.Services;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeeJet.Bot.Commands.Sources
{
    internal class SteamCommandSource : ICommandSource
    {
        private readonly SteamAPIService _steamAPI;

        public SteamCommandSource(SteamAPIService steamAPI)
        {
            _steamAPI = steamAPI;
        }

        internal static async Task JoinGamePressed(SocketMessageComponent component)
        {
            var match = new Regex("join-game-([a-zA-Z0-9\\s]*)-------([a-zA-Z0-9-\\s]*)").Match(component.Data.CustomId);
            if (match.Success)
            {
                var gameName = match.Groups[1].Value;
                var category = match.Groups[2].Value;
                var channel = await GetGameChannel(component.Message, gameName, category);
                await GameManagementCommandSource.GivePremissionToJoinChannel(component.User, channel);
                await component.DeferAsync(true);
            }
        }

        private static async Task<SocketTextChannel> GetGameChannel(IUserMessage message, string gameName, string categoryName)
        {
            var textChannels = ((SocketTextChannel)message.Channel).Guild.Channels.OfType<SocketTextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase) && b.Category.Name == categoryName.Replace("-", " "));
            return gameChannel;
        }

        public SlashCommandExecutedHandler GetCommandHandler(string commandName, SocketSlashCommand socketSlashCommand)
        {
            switch (commandName)
            {
                case "sync-steam":
                    return new SyncSteamCommandHandler(socketSlashCommand, _steamAPI);
            }
            return null;
        }

        public string[] GetCommandNames()
        {
            return new string[] { "sync-steam" };
        }

        public async Task RegisterCommands(IGuild guild)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("sync-steam");
            guildCommand.WithDescription("Sync steam library with channels");
            guildCommand.AddOption("steamid", ApplicationCommandOptionType.String, "Id of steamuser", isRequired: true);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }
    }
}
