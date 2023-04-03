using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeJet.Bot.Services
{
    public class GameInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoverImage { get;  set; }
        public string[] Urls { get; set; }
        public string IGDBUrl { get; set; }
    }
}
