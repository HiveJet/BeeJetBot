namespace BeeJet.Storage.Interfaces
{
    public interface IEchoMessage
    {
        int Id { get; set; }
        string Message { get; set; }
        ulong UserId { get; set; }
        ulong GuildId { get; set; }
    }
}
