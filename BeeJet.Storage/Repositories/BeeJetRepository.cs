using BeeJet.Storage.Databases;
using BeeJet.Storage.Interfaces;

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
        public Lazy<ISteamIdDiscordUserDb> SteamIdDiscordUserDb => new Lazy<ISteamIdDiscordUserDb>(() => new SteamIdDiscordUserDb(_database));
    }
}
