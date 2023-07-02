using BeeJet.Storage.Interfaces;

namespace BeeJet.Storage.Entities
{
    internal class EchoMessage : IEchoMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
    }
}
