using BeeJet.Bot.Data.Entities;
using LiteDB;

namespace BeeJet.Bot.Data.Databases
{
    public class EchoMessageDb : BaseDb<EchoMessage>, IEchoMessageDb
    {
        public EchoMessageDb(ILiteDatabase database) : base(database)
        {
        }

        protected override string ProvideCollectionName() => "echos";

        protected override void EnsureIndexes()
        {
            Collection.EnsureIndex(x => x.Id);
            Collection.EnsureIndex(x => x.UserId);
        }

        public EchoMessage GetLatestEcho()
        {
            return Collection.Query().OrderByDescending(x => x.Id).FirstOrDefault();
        }

        int IEchoMessageDb.Add(EchoMessage message) => Add(message);

        public bool Remove(EchoMessage message) => Remove(message);
    }
}
