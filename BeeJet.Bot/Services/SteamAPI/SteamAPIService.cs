using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Bot.Services.SteamAPI
{
    public class SteamAPIService
    {
        private SteamWebInterfaceFactory _webInterfaceFactory;
        public SteamAPIService(string apiKey)
        {
            _webInterfaceFactory = new SteamWebInterfaceFactory(apiKey);
        }

        public async Task<string[]> GetRecentlyPlayedGames(ulong userId)
        {
            var steamInterface = _webInterfaceFactory.CreateSteamWebInterface<PlayerService>(new HttpClient());
            var playerGames = (await steamInterface.GetRecentlyPlayedGamesAsync(userId)).Data;
            return playerGames.RecentlyPlayedGames.Select(b=> b.Name).ToArray();
        }

        public async Task<string[]> GetGamesFromSteamUser(ulong userId)
        {
            var steamInterface = _webInterfaceFactory.CreateSteamWebInterface<PlayerService>(new HttpClient());
            var playerGames = (await steamInterface.GetOwnedGamesAsync(userId, true, true)).Data;
            return playerGames.OwnedGames.Select(b => b.Name).ToArray();
        }
    }
}
