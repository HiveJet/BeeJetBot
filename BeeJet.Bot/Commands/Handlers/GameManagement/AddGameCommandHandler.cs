using BeeJet.Bot.Commands.Sources;
using Discord;
using Discord.WebSocket;

namespace BeeJet.Bot.Commands.Handlers.GameManagement
{
    internal class AddGameCommandHandler : SlashCommandExecutedHandler
    {
        public AddGameCommandHandler(SocketSlashCommand context) : base(context)
        {
        }

        internal override async Task SlashCommandExecuted()
        {
            var gameName = (string)Context.Data.Options.First().Value;
            await AddGameAsync(gameName);
        }

        public async Task AddGameAsync(string game)
        {
            await AddAdminRoleIfNeeded();

            if (Guild.Channels.Any(b => b.Name.Equals(game, StringComparison.OrdinalIgnoreCase)))
            {
                await Context.RespondAsync($"This game already has a channel", ephemeral: true);
                return;
            }

            ulong roleId = GetAdminRoleId();
            if (!(User as IGuildUser).RoleIds.Contains(roleId))
            {
                await Context.RespondAsync("To add a game you need the role 'BeeJetBotAdmin'", ephemeral: true);
                return;
            }

            await AddToGameListChannel(game);

            var channel = await Guild.CreateTextChannelAsync(game.Trim().Replace(" ", "-"));
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            await channel.AddPermissionOverwriteAsync(Guild.EveryoneRole, permissionOverrides);
            await channel.SendMessageAsync($"This is the channel for {game}");

            await Context.RespondAsync($"Channel created", ephemeral: true);
        }

        private async Task AddToGameListChannel(string game)
        {
            var gameListChannel = await AddOrGetGameListChannel();
            var builder = new ComponentBuilder().WithButton("Join", GameManagementCommandSource.JointButtonId, ButtonStyle.Success).WithButton("Leave", GameManagementCommandSource.LeaveButtonId, ButtonStyle.Danger);
            var message = await gameListChannel.SendMessageAsync($"Click to join channel for {game}", components: builder.Build());
        }

        private async Task<ITextChannel> AddOrGetGameListChannel()
        {
            if (!Guild.Channels.Any(c => c.Name.Equals(GameManagementCommandSource.ChannelName, StringComparison.OrdinalIgnoreCase)))
            {
                var gameListChannel = await Guild.CreateTextChannelAsync(GameManagementCommandSource.ChannelName);
                var permissionOverrides = new OverwritePermissions(sendMessages: PermValue.Deny, sendMessagesInThreads: PermValue.Deny);
                await gameListChannel.AddPermissionOverwriteAsync(Guild.EveryoneRole, permissionOverrides);
                return gameListChannel;
            }
            else
            {
                var textChannels = Guild.Channels.OfType<ITextChannel>();
                return textChannels.SingleOrDefault(c => c.Name.Equals(GameManagementCommandSource.ChannelName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
