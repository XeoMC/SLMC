using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class Condition
    {
        public Rule[] rules;
        public dynamic value;
        public bool Result(Profile p)
        {
            bool result = false;
            foreach (Rule r in this.rules)
                result = r.isAllowed(p);
            return result;
        }
    }
}
