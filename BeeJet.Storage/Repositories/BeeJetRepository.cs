using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeJet.Storage.Databases;
using BeeJet.Storage.Interfaces;
using LiteDB;

namespace BeeJet.Storage.Repositories
{
    public class BeeJetRepository : IBeeJetRepository
    {
        private readonly ILiteDatabase _database;

        public BeeJetRepository(ILiteDatabase database)
        {
            _database = database;
        }

        public Lazy<IEchoMessageDb> EchoMessageDb => new Lazy<IEchoMessageDb>(() => new EchoMessageDb(_database));
    }
}
