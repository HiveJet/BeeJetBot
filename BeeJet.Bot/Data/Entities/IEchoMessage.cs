namespace BeeJet.Bot.Data.Entities
{
    public interface IEchoMessage
    {
        int Id { get; set; }
        string Message { get; set; }
        ulong UserId { get; set; }
    }
}
