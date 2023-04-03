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


        public async Task<GameInfo> GetGameInfoAsync(string gameName)
        {
            var games = await _igdbClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: $"fields id,name,url,cover.*,summary,websites.*; search \"{gameName}\"; limit 2;");
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
                string[] urls = game.Websites.Values.Where(b => b.Category == WebsiteCategory.Steam || b.Category == WebsiteCategory.EpicGames || b.Category == WebsiteCategory.GOG).Select(b => b.Url).ToArray();
                string coverUrl = null;
                if (game.Cover != null && !string.IsNullOrWhiteSpace(game.Cover.Value.ImageId))
                {
                    coverUrl = ImageHelper.GetImageUrl(game.Cover.Value.ImageId, ImageSize.CoverBig);
                }
                return new GameInfo()
                {
                    Name = game.Name,
                    Description = game.Summary,
                    Urls = urls,
                    CoverImage = coverUrl,
                    IGDBUrl = game.Url
                };
            }
        }
    }
}
