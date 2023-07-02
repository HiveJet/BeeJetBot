using BeeJet.Storage.Entities;
using BeeJet.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Storage.Databases
{
    internal class ButtonContextDb : BaseDb<IButtonContext, ButtonContext>, IButtonContextDb
    {
        public ButtonContextDb(ILiteDatabase database)
            : base(database)
        {
        }

        protected override void EnsureIndexes()
        {
            Collection.EnsureIndex(b => b.Id);
            Collection.EnsureIndex(b => b.MessageId);
        }

        public void CreateNewButtonContext(ulong messageId, string customIdButton, object context)
        {
            var buttonContext = Create();
            buttonContext.MessageId = messageId;
            buttonContext.CustomButtonId = customIdButton;
            buttonContext.HandlerContext = context;
            Add(buttonContext);
        }

        public ICollection<IButtonContext> GetButtonContexts(ulong messageId)
        {
            return Collection.Query().Where(b => b.MessageId == messageId).ToList().OfType<IButtonContext>().ToList();
        }


        protected override string ProvideCollectionName() => "buttoncontexts";

        public IButtonContext GetButtonContext(ulong messageId, string customButtonId)
        {
            return Collection.Query().Where(b => b.MessageId == messageId && b.CustomButtonId == customButtonId).FirstOrDefault();
        }
    }
}
