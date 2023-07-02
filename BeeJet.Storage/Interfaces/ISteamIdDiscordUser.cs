namespace BeeJet.Storage.Interfaces
{
    internal interface ISteamIdDiscordUser
    {
        string DiscordId { get; set; }
        int Id { get; set; }
        string SteamId { get; set; }
    }
}