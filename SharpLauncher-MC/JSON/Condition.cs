using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class Condition
    {
        public List<Rule> rules;
        public object value;
        public bool Result(Profile p)
        {
            bool result = false;
            foreach (Rule r in this.rules)
                if (r.Result(p))
                    if (r.action == Action.allow) result = true;
                    else if (r.action == Action.disallow) result = false;
            return result;
        }
    }
}
