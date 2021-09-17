using MojangAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class ClassicVersion
    {
        // MC Version
        public List<List<object>> arguments;
        public File assetIndex;
        public string assets;
        public uint complianceLevel;
        public List<File> downloads;
        public string id;
        public List<Library> libraries;
        public LoggingEntry logging;
        public string mainClass;
        public uint minimumLauncherVersion;
        public string releaseTime;
        public string time;
        public string type;

        //OLD
        public string minecraftArguments;

        public Profile ToProfile(string name = "Test",
            string icon = "Grass",
            string javaPath = "",
            bool sharedAssets = true,
            string assetsPath = "",
            bool addSharedJavaArgs = true,
            string javaArgs = "",
            bool sharedSession = true,
            Session session = null)
        {
            var p = (Profile)this;
            p.name = name;
            p.icon = icon;
            p.javaPath = javaPath;
            p.sharedAssets = sharedAssets;
            p.assetsPath = assetsPath;
            p.addSharedJavaArgs = addSharedJavaArgs;
            p.javaArgs = javaArgs;
            p.sharedSession = sharedSession;
            p.session = session;
            return p;
        }
    }
}
