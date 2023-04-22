using BeeJet.Bot.Data.Entities;

namespace BeeJet.Bot.Data.Databases
{
    public interface IEchoMessageDb
    {
        public int Add(EchoMessage message);
        public bool Remove(EchoMessage message);
        public EchoMessage GetLatestEcho();
    }
}
