using BeeJet.Bot.Interfaces;
using BeeJet.Bot.Managers;
using Discord;
using System.Text.RegularExpressions;

namespace BeeJet.Bot.Commands.Handlers
{
    public class MessageGameManagementHandler : BaseGameManagementHandler
    {
        private IUserMessage _userMessage;

        public MessageGameManagementHandler(IUserMessage message, IGuildManager guildManager)
            :base (guildManager)
        {
            _userMessage = message;
        }

        public override IMessageChannel MessageChannel => _userMessage.Channel;

        public override IGuildUser User => (IGuildUser)_userMessage.Author;

        public async Task<bool> JoinGamePressed()
        {
            var gameName = DetermineGameName();
            if (string.IsNullOrEmpty(gameName))
            {
                await MessageChannel.SendMessageAsync("Invalid game name");
                return false;
            }

            var gameChannel = await GuildManager.GetTextChannelAsync(gameName);
            if (gameChannel is null)
            {
                await MessageChannel.SendMessageAsync("Game channel does not exist!");
                return false;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
            await gameChannel.AddPermissionOverwriteAsync(User, permissionOverrides);
            await gameChannel.SendMessageAsync($"Welcome <@{User.Id}>");
            return true;
        }

        //TODO: Messag moet wel Click to leave channel for zijn.....
        public async Task<bool> LeaveGamePressed()
        {
            var gameName = DetermineGameName();
            if (!string.IsNullOrEmpty(gameName))
            {
                await MessageChannel.SendMessageAsync("Invalid game name");
                return false;
            }

            var gameChannel = await GuildManager.GetTextChannelAsync(gameName);
            if (gameChannel is null)
            {
                await MessageChannel.SendMessageAsync("Game channel does not exist!");
                return false;
            }

            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
            await gameChannel.AddPermissionOverwriteAsync(User, permissionOverrides);
            await gameChannel.SendMessageAsync($"<@{User.Id}> has left the channel");
            return true;
        }

        private string DetermineGameName()
        {
            var regex = new Regex(@"Click to join channel for ([a-zA-Z0-9\s]*)");
            var matching = regex.Match(_userMessage.Content);
            if (matching.Success)
            {
                return matching.Groups[1].Value.Trim().Replace(" ", "-");
            }
            return string.Empty;
        }
    }
}
