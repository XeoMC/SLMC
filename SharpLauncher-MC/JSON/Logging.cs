using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class LoggingEntry
    {
        public Logging client;
    }
    public class Logging
    {
        public string argument;
        public File file;
        public string type;
    }
}
