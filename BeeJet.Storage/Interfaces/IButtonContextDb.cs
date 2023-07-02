namespace BeeJet.Storage.Interfaces
{
    public interface IButtonContextDb
    {
        void CreateNewButtonContext(ulong messageId, string customIdButton, object context);
        IButtonContext GetButtonContext(ulong messageId, string customButtonId);
        ICollection<IButtonContext> GetButtonContexts(ulong messageId);
    }
}