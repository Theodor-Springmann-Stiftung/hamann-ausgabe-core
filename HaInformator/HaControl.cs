using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaInformator
{
    public abstract class HaControl
    {
        public enum HaControlResult
        {
            OK,
            Error
        }

        public string Description { get; internal set; }
        public Logger Logger { get; internal set; }

        public abstract HaControlResult Act(string fiepath);
    }
}
