namespace BeeJet.Storage.Interfaces
{
    public interface IButtonContext
    {
        public int Id { get; set; }
        public ulong MessageId { get; set; }
        public string CustomButtonId { get; set; }
        public object? HandlerContext { get; set; }
    }
}