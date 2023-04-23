using BeeJet.Storage.Entities;

namespace BeeJet.Storage.Interfaces
{
    public interface IEchoMessageDb
    {
        public IEchoMessage Create();
        public int Add(IEchoMessage message);
        public bool Remove(IEchoMessage message);
        public IEchoMessage GetLatestEcho(ulong guildId);
    }
}
