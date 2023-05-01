namespace BeeJet.Storage.Interfaces
{
    public interface IButtonContextDb
    {
        void CreateNewButtonContext(ulong messageId, string customIdButton, object context);
        IButtonContext GetButtonContextForMessageIdAndCustomId(ulong id, string customButtonId);
        ICollection<IButtonContext> GetButtonContextsForMessageId(ulong messageId);
    }
}