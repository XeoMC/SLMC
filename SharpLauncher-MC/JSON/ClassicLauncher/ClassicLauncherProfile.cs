using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON.LauncherProfiles
{
    public class ClassicLauncherProfile
    {
        public string gameDir;
        public string javaDir;
        public string javaArgs;
        public string icon;
        public string lastUsed; // not going to use it, too lazy...
        public string lastVersionId;
        public string name;
        public Resolution resolution;
        public string type;
        public Profile Convert()
        {
            Profile p = new Profile { javaArgs = this.javaArgs, addSharedJavaArgs = this.javaArgs == "" };
            return p;
        }
    }
}
