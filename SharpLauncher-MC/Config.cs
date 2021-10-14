using MojangAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpLauncher_MC
{
    public class Config
    {
        public static Config i;
        public bool largeMode = false;
        public Size resolution = new Size(280,420);
        public double settingsSize = 1.5;
        public string minecraftPath;
        public string javaPath;
        public string javaArgs = "-Xmx2G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M";
        public string externalUnpacker;
        public static List<Session> sessions = new List<Session>();
        public Config()
        {

        }
        public static void Save()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC");
            if (Config.i == null) Config.i = new Config();
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC/SLMC.json", JsonConvert.SerializeObject(Config.i));
        }
        public static void Load()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC/SLMC.json"))
                Config.i = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.SLMC/SLMC.json"));
            else 
                Config.Save();
        }
    }
}
