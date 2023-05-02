using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeJet.Storage.Interfaces;

namespace BeeJet.Storage.Entities
{
    internal class SteamIdDiscordUser : ISteamIdDiscordUser
    {
        public int Id { get; set; }
        public string DiscordId { get; set; } = "";
        public string SteamId { get; set; } = "";
    }
}
