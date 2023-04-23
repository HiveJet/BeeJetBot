namespace BeeJet.Storage.Interfaces
{
    public interface IBeeJetRepository
    {
        Lazy<IEchoMessageDb> EchoMessageDb { get; }
    }
}
