namespace BeeJet.Storage.Interfaces
{
    public interface ISteamIdDiscordUserDb
    {
        void SetDiscordSteamMapping(string discordId, string steamId);
        string? GetSteamId(string discordId);
    }
}