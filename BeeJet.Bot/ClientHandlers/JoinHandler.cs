﻿using Discord.Commands;
using Discord.WebSocket;

namespace BeeJet.Bot.ClientHandlers
{
    public class JoinHandler : BaseClientHandler
    {
        public JoinHandler(DiscordSocketClient client, CommandService service, IServiceProvider serviceProvider)
            : base(client, service, serviceProvider)
        {
        }

        public async Task UserJoinedAsync(SocketGuildUser arg)
        {
            await arg.Guild.SystemChannel.SendMessageAsync($"Welcome to the server {arg.DisplayName}");
        }
    }
}
