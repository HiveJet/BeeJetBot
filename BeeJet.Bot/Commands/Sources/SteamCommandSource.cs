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

        public SlashCommandExecutedHandler GetCommandHandler(string commandName, SocketSlashCommand socketSlashCommand)
        {
             switch (commandName)
                {
                    case "sync-steam":
                        return new SyncSteamCommandHandler(socketSlashCommand, _steamAPI);
                }
                return null;
            
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
            guildCommand.AddOption("steamid", ApplicationCommandOptionType.Integer, "Id of steamuser", isRequired: true);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }
    }
}
