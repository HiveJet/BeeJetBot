using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Bot.Data.Entities
{
    public class EchoMessage : IEchoMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } 
        public ulong UserId { get; set; }
    }
}
