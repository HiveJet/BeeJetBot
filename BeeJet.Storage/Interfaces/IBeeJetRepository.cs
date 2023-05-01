namespace BeeJet.Storage.Interfaces
{
    public interface IBeeJetRepository
    {
        Lazy<IEchoMessageDb> EchoMessageDb { get; }
        Lazy<IButtonContextDb> ButtonContextDb { get; }
    }
}
