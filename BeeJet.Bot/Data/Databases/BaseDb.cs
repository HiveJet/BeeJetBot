using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace BeeJet.Bot.Data.Databases
{
    public abstract class BaseDb<TEntity> where TEntity : class
    {
        private readonly ILiteDatabase _database;

        protected ILiteCollection<TEntity> Collection { get; }
        protected abstract string ProvideCollectionName();

        public BaseDb(ILiteDatabase database)
        {
            _database = database;
            Collection = database.GetCollection<TEntity>(ProvideCollectionName());

            EnsureIndexes();
        }

        protected abstract void EnsureIndexes();

        protected BsonValue Add(TEntity entity)
        {
            return Collection.Insert(entity);
        }

        protected bool Remove(BsonValue id)
        {
            return Collection.Delete(id);
        }
    }
}
