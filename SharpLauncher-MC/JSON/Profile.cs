using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    class Profile
    {
        public List<List<dynamic>> arguments;
        public File assetIndex;
        public string assets;
        public uint complianceLevel;
        public List<File> downloads;
        public string id;
        public List<File> libraries;
        public Dictionary<string, Logging> logging;
        public string mainClass;
        public uint minimumLauncherVersion;
        public string releaseTime;
        public string time;
        public string type;

        //OLD
        public string minecraftArguments;

    }
}
