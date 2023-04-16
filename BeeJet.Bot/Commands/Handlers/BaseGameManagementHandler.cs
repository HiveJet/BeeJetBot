using BeeJet.Bot.Interfaces;
using Discord;

namespace BeeJet.Bot.Commands.Handlers
{
    public abstract class BaseGameManagementHandler : BaseHandler
    {
        internal static readonly string ChannelName = "Game-channels";
        internal const string JointButtonId = "join-game-id";
        internal const string LeaveButtonId = "leave-game-id";

        public BaseGameManagementHandler(IGuildManager guildManager, IUser user)
            : base(guildManager, user)
        {            
        }

        public async Task AddGameAsync(string gameName)
        {
            if(!GuildManager.IsAdmin(User))
            {
                await MessageChannel.SendMessageAsync($"You don't have permission to create game channels");
            }

            var gameChannelExists = await GuildManager.ChannelExistsAsync(gameName);
            if (gameChannelExists)
            {
                await MessageChannel.SendMessageAsync($"This game already has a channel");
                return;
            }

            var createdChannel = await GuildManager.CreateGameChannelAsync(gameName);
            if(createdChannel is null)
            {
                await MessageChannel.SendMessageAsync($"Unable to create game channel for {gameName}");
                return;
            }
            await createdChannel.SendMessageAsync($"This is the channel for {gameName}");

            var gameListChannel = await GuildManager.GetMainGameListChannelAsync();
            if(gameListChannel is null)
            {
                gameListChannel = await GuildManager.CreateMainGameListChannel();
            }
            await CreateNewGameChannelNotificationAsync(gameName, gameListChannel);
        }

        private async Task CreateNewGameChannelNotificationAsync(string gameName, ITextChannel gameListChannel)
        {
            var builder = new ComponentBuilder()
                .WithButton("Join", JointButtonId, ButtonStyle.Success)
                .WithButton("Leave", LeaveButtonId, ButtonStyle.Danger);
            await gameListChannel.SendMessageAsync($"Click to join channel for {gameName}", components: builder.Build());
        }
    }
}
