namespace BeeJet.Storage.Interfaces
{
    public interface ISteamIdDiscordUserDb
    {
        void AddOrUpdateMapping(string discordId, string steamId);
        string? GetSteamId(string discordId);
    }
}