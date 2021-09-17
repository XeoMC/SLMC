using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class Library
    {
        public Download downloads;
        public string name;
        public Natives natives;
        public Extract extract;
        public List<Rule> rules;
    }
}
