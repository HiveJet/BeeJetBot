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

        public void SetDiscordSteamMapping(string discordId, string steamId)
        {
            var currentMapping = DetermineMapping(discordId);
            if (currentMapping == null)
            {
                AddMapping(discordId, steamId);
            }
            else
            {
                UpdateMapping(steamId, currentMapping);
            }
        }

        private SteamIdDiscordUser DetermineMapping(string discordId)
        {
            return Collection.Query().Where(b => b.DiscordId == discordId).SingleOrDefault();
        }

        private void UpdateMapping(string steamId, SteamIdDiscordUser currentMapping)
        {
            currentMapping.SteamId = steamId;
            Collection.Update(currentMapping);
        }

        private void AddMapping(string discordId, string steamId)
        {
            var mapping = Create();
            mapping.DiscordId = discordId;
            mapping.SteamId = steamId;
            Add(mapping);
        }

        public string? GetSteamId(string discordId)
        {
            return DetermineMapping(discordId)?.SteamId;
        }

    }
}
