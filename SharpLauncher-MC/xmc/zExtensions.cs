using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmc
{
    public static class zExtensions
    {
        public static TimeSpan Multiply(this TimeSpan ts, double multiplier)
        {
            ts = new TimeSpan((long)(ts.Ticks * multiplier));
            return ts;
        }
    }
}
