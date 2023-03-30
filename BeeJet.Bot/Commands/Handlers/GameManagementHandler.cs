using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BeeJet.Bot.Commands.Handlers
{
    public class GameManagementHandler : BaseHandler
    {
        internal static readonly string ChannelName = "Game-channels";
        internal const string JointButtonId = "join-game-id";
        internal const string LeaveButtonId = "leave-game-id";

        public GameManagementHandler(SocketCommandContext context)
            : base(context)
        {
        }

        public async Task AddGameAsync(string game)
        {
            await AddAdminRoleIfNeeded();

            if (Context.Guild.Channels.Any(b => b.Name.Equals(game, StringComparison.OrdinalIgnoreCase)))
            {
                await Context.Channel.SendMessageAsync($"Voor deze game bestaat al een kanaal");
                return;
            }

            ulong roleId = GetAdminRoleId();
            if (!(Context.User as IGuildUser).RoleIds.Contains(roleId))
            {
                await Context.Channel.SendMessageAsync("Voor het toevoegen van een game heb je de role 'BeeJetBotAdmin' nodig");
                return;
            }

            await AddToGameListChannel(game);

            var channel = await Context.Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"));
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permissionOverrides);
            await channel.SendMessageAsync($"Dit is een kanaal voor {game}");

        }

        private async Task AddToGameListChannel(string game)
        {
            var gameListChannel = await AddOrGetGameListChannel();
            var builder = new ComponentBuilder().WithButton("Join", JointButtonId, ButtonStyle.Success).WithButton("Leave", LeaveButtonId, ButtonStyle.Danger);
            var message = await gameListChannel.SendMessageAsync($"Click to join channel for {game}", components: builder.Build());
        }

        private async Task<ITextChannel> AddOrGetGameListChannel()
        {
            if (!Context.Guild.Channels.Any(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase)))
            {
                var gameListChannel = await Context.Guild.CreateTextChannelAsync(ChannelName);
                var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
                await gameListChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permissionOverrides);
                return gameListChannel;
            }
            else
            {
                var textChannels = Context.Guild.Channels.OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static async Task JoinGamePressed(IUserMessage message, SocketUser user)
        {
            if (TryGetGameName(message, out string gameName))
            {

                ITextChannel? gameChannel = await GetGameChannel(message, gameName);
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Allow);
                    await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                    await gameChannel.SendMessageAsync($"Welkom <@{user.Id}>");
                }
            }
        }

        public static async Task LeaveGamePressed(IUserMessage message, SocketUser user)
        {
            if (TryGetGameName(message, out string gameName))
            {
                ITextChannel? gameChannel = await GetGameChannel(message, gameName);
                if (gameChannel != null)
                {
                    var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Inherit);
                    await gameChannel.AddPermissionOverwriteAsync(user, permissionOverrides);
                    await gameChannel.SendMessageAsync($"Helaas heeft <@{user.Id}> dit channel verlaten");
                }
            }
        }

        private static bool TryGetGameName(IUserMessage message, out string gameName)
        {
            var regex = new Regex(@"Click to join channel for ([a-zA-Z\s]*)");
            var matching = regex.Match(message.Content);
            if (matching.Success)
            {
                gameName = matching.Groups[1].Value.Trim().Replace(" ", "-");
                return true;
            }
            gameName = null;
            return false;
        }

        private static async Task<ITextChannel> GetGameChannel(IUserMessage message, string gameName)
        {
            var textChannels = (await (message.Channel as IGuildChannel).Guild.GetChannelsAsync()).OfType<ITextChannel>();
            var gameChannel = textChannels.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));
            return gameChannel;
        }
    }
}
