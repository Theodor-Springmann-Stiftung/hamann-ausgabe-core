using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HaLive.Models
{
    public abstract class MenuItem
    {
        public string FriendlyName { get; set; }
        public string DefaultRoute { get; set; }
        public bool Active { get; set; } = false;
    }
}
