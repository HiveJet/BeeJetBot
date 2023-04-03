using IGDB;
using IGDB.Models;

namespace BeeJet.Bot.Services
{
    public class IGDBService
    {
        private IGDBClient _igdbClient;
        public IGDBService(string clientId, string clientSecret)
        {
            _igdbClient = new IGDBClient(clientId, clientSecret);
        }


        public async Task<GameInfo> GetGameInfo(string gameName)
        {
            var games = await _igdbClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: $"fields *; search \"{gameName}\"; limit 2;");
            var game = games.FirstOrDefault(b => b.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));
            if (game == null && games.Count() == 1)
            {
                game = games.FirstOrDefault();
            }
            if (game == null)
            {
                return null;
            }
            else
            {
                return new GameInfo()
                {
                    Name = game.Name,
                    Description = game.Summary
                };
            }
        }
    }
}
