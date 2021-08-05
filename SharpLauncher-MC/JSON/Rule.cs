using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    enum Action
    {
        allow,
        disallow
    }
    class Rule
    {
        public Action action;
        public Dictionary<string, bool> features;
        public OS os;
    }
}
