using MojangAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class Profile : ClassicVersion
    {
        // SLMC Profile
        public string name = "";
        public string icon = "Grass";
        public string assetsPath;
        public bool sharedAssets = true;
        public Session session;
        public bool sharedSession = true;
        public string javaArgs = "";
        public bool addSharedJavaArgs = true;
        public string javaPath = "";
        public List<string> sharedData; // files*
        public Resolution resolution;
        public uint sortPosition; // in launcher

        public static List<string> getVersions()
        {
            return new List<string>(Directory.GetFiles($"{Config.i.minecraftPath}\\Profiles"));
        }

        public string getArguments()
        {
            string args = "";
            if (this.javaPath == "")
                args += $"{Config.i.javaPath}";
            else
                args += $"{javaPath}";
            args += "\\javaw.exe ";
            args += "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump "; // looks like hardcoded thing
            args += "\"-Dos.name=Windows 10\" "; // idk where to find it
            args += $"-Dos.version={Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor} ";
            args += $"-Xss1M "; // idk where to find it too
            var nativesPath = $"{Config.i.minecraftPath}\\{Guid.NewGuid()}";
            args += $"-Djava.library.path={nativesPath} ";
            args += $"-Dminecraft.launcher.brand={Assembly.GetExecutingAssembly().GetName().Name} ";
            args += $"-Dminecraft.launcher.version={Assembly.GetExecutingAssembly().GetName().Version} ";
            args += $"-cp ";

            foreach(Library l in this.libraries)
            {
                foreach (Rule r in l.rules) 
                    if(!r.Result(this)) 
                        continue;
                args += $"{Config.i.minecraftPath}\\libraries\\{l.downloads.artifact.path.Replace("/", "\\")}; ";
            }
            args.Remove(args.LastIndexOf(";"));

            if(this.addSharedJavaArgs)
            args += $"{Config.i.javaArgs} ";
            args += $"{javaArgs} ";
            args += this.logging.client.argument.Replace("${path}", $"{Config.i.minecraftPath}\\assets\\log_configs\\{this.logging.client.file.id} {this.mainClass} ");
            foreach (object obj in arguments)
            {
                if (obj is string)
                {
                    args += replaceArg((string)obj);
                }
                else if (obj is Condition)
                {
                    if (((Condition)obj).Result(this))
                        if (((Condition)obj).value is string)
                            args += replaceArg((string)((Condition)obj).value);
                        else if (((Condition)obj).value is string[])
                            args += replaceArg((string[])((Condition)obj).value);
                        else
                            Console.WriteLine("Something's wrong. Condition value is not a string or string[]");
                    else
                        Console.WriteLine("Condition result is false");
                }
                else
                    Console.WriteLine("Something's wrong. Argument is not a string or condition");
            }
            return args;
        }
        public string replaceArg(string arg)
        {
            return arg;
        }
        public string replaceArg(string[] args)
        {
            string final = "";
            var lSession = this.sharedSession ? MainWindow.CurrentSession : this.session;
            foreach (string s in args)
            {
                string temp = s
                    .Replace("${auth_player_name}", lSession.Username)
                    .Replace("${version_name}", this.id)
                    .Replace("${game_directory}", $"{Config.i.minecraftPath}\\{this.name}")
                    .Replace("${assets_root}", this.sharedAssets ? $"{Config.i.minecraftPath}\\assets" : this.assetsPath)
                    .Replace("${assets_index_name}", this.assetIndex.id)
                    .Replace("${auth_uuid}", lSession.UUID)
                    .Replace("${auth_access_token}", lSession.AccessToken)
                    .Replace("${user_type}", "microsoft") // Temporary hardcoded, sorry!
                    .Replace("${version_type}", this.type);
                final += $"{temp} ";
            }
            return final;
        }

    }
}
