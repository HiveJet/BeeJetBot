using BeeJet.Storage.Entities;
using BeeJet.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Storage.Databases
{
    internal class SteamIdDiscordUserDb : BaseDb<ISteamIdDiscordUser, SteamIdDiscordUser>, ISteamIdDiscordUserDb
    {
        public SteamIdDiscordUserDb(ILiteDatabase database) : base(database)
        {
        }

        protected override void EnsureIndexes()
        {
            Collection.EnsureIndex(b => b.Id);
            Collection.EnsureIndex(b => b.SteamId);
            Collection.EnsureIndex(b => b.DiscordId);
        }

        protected override string ProvideCollectionName() => "steamdiscordmappings";

        public void AddOrUpdateMapping(string discordId, string steamId)
        {
            var currentMapping = Collection.Query().Where(b => b.DiscordId == discordId).SingleOrDefault();
            if (currentMapping == null)
            {
                var mapping = Create();
                mapping.DiscordId = discordId;
                mapping.SteamId = steamId;
                Add(mapping);
            }
            else
            {
                currentMapping.SteamId = steamId;
                Collection.Update(currentMapping);
            }
        }

        public string? GetSteamId(string discordId)
        {
            return Collection.Query().Where(b => b.DiscordId == discordId).SingleOrDefault()?.SteamId;
        }

    }
}
