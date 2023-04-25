namespace BeeJet.Storage.Interfaces
{
    public interface IBeeJetRepository
    {
        Lazy<IEchoMessageDb> EchoMessageDb { get; }
        Lazy<ISteamIdDiscordUserDb> SteamIdDiscordUserDb { get; }
    }
}
