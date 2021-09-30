using MojangAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON.ClassicLauncher
{
    public class ClassicVersion
    {
        // MC Version
        public Arguments arguments;
        public File assetIndex;
        public string assets;
        public uint complianceLevel;
        public VersionDownloads downloads;
        public string id;
        //java version
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
            Profile profile = JsonConvert.DeserializeObject<Profile>(JsonConvert.SerializeObject(this));

            profile.name = name;
            profile.icon = icon;
            profile.javaPath = javaPath;
            profile.sharedAssets = sharedAssets;
            profile.assetsPath = assetsPath;
            profile.addSharedJavaArgs = addSharedJavaArgs;
            profile.javaArgs = javaArgs;
            profile.sharedSession = sharedSession;
            profile.session = session;
            return profile;
        }
    }
}
