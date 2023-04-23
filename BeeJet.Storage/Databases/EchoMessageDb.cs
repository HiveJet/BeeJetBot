using BeeJet.Storage.Entities;
using BeeJet.Storage.Interfaces;
using LiteDB;

namespace BeeJet.Storage.Databases
{
    internal class EchoMessageDb : BaseDb<IEchoMessage, EchoMessage>, IEchoMessageDb
    {
        internal EchoMessageDb(ILiteDatabase database) : base(database)
        {
        }

        protected override string ProvideCollectionName() => "echos";

        protected override void EnsureIndexes()
        {
            Collection.EnsureIndex(x => x.Id);
            Collection.EnsureIndex(x => x.GuildId);
        }
        
        public IEchoMessage GetLatestEcho(ulong guildId)
        {
            return Collection.Query().Where(x => x.GuildId == guildId).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        IEchoMessage IEchoMessageDb.Create() => Create();

        int IEchoMessageDb.Add(IEchoMessage message) => Add(message);

        bool IEchoMessageDb.Remove(IEchoMessage message) => Remove(message.Id);
    }
}
