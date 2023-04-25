using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeJet.Storage.Interfaces;

namespace BeeJet.Storage.Entities
{
    internal class ButtonContext : IButtonContext
    {
        public int Id { get; set; }
        public ulong MessageId { get; set; }
        public string CustomButtonId { get; set; } = "";
        public object? HandlerContext { get; set; }
    }
}
