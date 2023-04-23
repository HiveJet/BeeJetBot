using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Storage.Databases
{
    internal abstract class BaseDb<TInterface, TEntity> where TEntity : class, TInterface, new() 
    {
        protected ILiteCollection<TEntity> Collection { get; }
        protected abstract string ProvideCollectionName();

        internal BaseDb(ILiteDatabase database)
        {
            Collection = database.GetCollection<TEntity>(ProvideCollectionName());

            EnsureIndexes();
        }

        protected abstract void EnsureIndexes();

        protected TInterface Create() => new TEntity();

        protected BsonValue Add(TInterface entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            if (entity is not TEntity)
            {
                throw new ArgumentException($"Invalid entity type, expected {typeof(TEntity)}, received {entity.GetType()}");
            }

            return Collection.Insert((TEntity)entity);
        }

        protected bool Remove(BsonValue id)
        {
            return Collection.Delete(id);
        }
    }
}
