using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HaLive.Models
{
    public class MenuSubMenuItem : MenuItem
    {
        public List<MenuPageItem> menuPages { get; set; }
    }
}
