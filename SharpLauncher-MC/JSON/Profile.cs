using MojangAPI.Model;
using Newtonsoft.Json;
using SharpLauncher_MC.JSON.ClassicLauncher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static List<string> GetVersions()
        {
            return new List<string>(Directory.GetFiles($"{Config.i.minecraftPath}/Profiles"));
        }
        [JsonIgnore]
        public List<Task> LibraryTasks = new List<Task>();

        public async void ExtractNatives(List<Library> libraries, string nativesPath)
        {
            foreach(Library l in libraries)
            {
                var osN = "natives-" + MainWindow.GetLauncherOSName().Replace("osx", "macos");
                if (l.natives.windows == osN)
                {
                    var saveLocation = $"{Config.i.minecraftPath}/libraries/{l.downloads.classifiers.natives_windows.path}";
                    await Task.WhenAll(l.downloads.classifiers.natives_windows.DownloadTask(saveLocation)); // now extract
                    Task t = new Task(() =>
                    {
                        if( String.IsNullOrEmpty(Config.i.externalUnpacker))
                        {
                            throw new NotImplementedException("Please, use external unpacker parameter for now.");
                        } else
                        {
                            var process = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = Config.i.externalUnpacker
                                        .Replace(@"${file}", saveLocation)
                                        .Replace(@"${path}", nativesPath)
                                }
                            };
//                            process.Start();
//                            process.WaitForExit();
                        }
                    });
                    await t.ContinueWith((a) =>
                    {
                        LibraryTasks.Remove(t);
                    });
                    LibraryTasks.Add(t);
                    t.Start();
                }
                else if (l.natives.linux == osN)
                {
                    var saveLocation = $"{Config.i.minecraftPath}/libraries/{l.downloads.classifiers.natives_linux.path}";
                    throw new NotImplementedException("Sorry, SLMC doesn't supports linux currently");
                }
                else if (l.natives.osx == osN)
                {
                    var saveLocation = $"{Config.i.minecraftPath}/libraries/{l.downloads.classifiers.natives_osx.path}";
                    throw new NotImplementedException("Sorry, SLMC doesn't supports osx currently");
                }
            }
        }
        public async void Start()
        {
            var cli = GetArguments();
            await Task.WhenAll(LibraryTasks);
        }
        public string GetArguments()
        {
            string t = "";
            if (this.javaPath == "")
                t += $"{Config.i.javaPath}";
            else
                t += $"{javaPath}";
            t += "/javaw.exe";

            string args = replaceArg(Path.GetFullPath(t));
            args += toArgs(arguments.jvm);
            var classpath = "";
            List<Library> natives = new List<Library>();
            foreach(Library l in this.libraries)
            {
                bool res = true;
                if (l.rules != null)
                foreach (Rule r in l.rules) 
                    if(!r.isAllowed(this)) 
                        res = false;
                if(res)
                {
                    string path = Path.GetFullPath($"{Config.i.minecraftPath}/libraries/{l.downloads.artifact.path}");
                    if(!classpath.Contains(path))
                    {
                        l.downloads.artifact.DownloadTask(path);
                        classpath += $"{path};";
                    }
                    if(l.natives != null)
                        natives.Add(l);
                }
            }
            classpath += $"{Path.GetFullPath($"{Config.i.minecraftPath}/versions/{this.id}/{this.id}.jar")} ";
            var nativesPath = Path.GetFullPath($"{Config.i.minecraftPath}/natives/{this.id}");
            args = args.Replace("${classpath}", classpath).Replace("${natives_directory}", nativesPath);
            ExtractNatives(natives, nativesPath);

            if (this.addSharedJavaArgs)
                args += $"{Config.i.javaArgs} ";

            args += $"{javaArgs} ";
            args += this.logging.client.argument.Replace("${path}", $"{Path.GetFullPath($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}")} {this.mainClass} ");
            this.logging.client.file.DownloadTask($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}");
            args += toArgs(arguments.game);
            return args;
        }

        private string toArgs(object[] list)
        {
            string args = "";
            foreach (object obj in list)
            {
                if (obj is string)
                {
                    args += replaceArg((string)obj);
                }
                else
                {
                    Condition condition = JsonConvert.DeserializeObject<Condition>(JsonConvert.SerializeObject(obj)); // really bad way to fix it...
                    if (condition != null)
                        if (condition.Result(this))
                            if (condition.value is string conditionString)
                                args += replaceArg(conditionString);
                            else if (condition.value is string[] conditionStringArray)
                                args += replaceArg(conditionStringArray);
                            else
                            {
                                Console.WriteLine($"Something's wrong. Condition value is not a string or a string[]:\n{JsonConvert.SerializeObject(obj)}");
                                args += replaceArg(JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject((object)condition.value)));
                            }
                        else
                            continue;
                    else
                        Console.WriteLine($"Something's wrong. Argument is not a string or a condition:\n{JsonConvert.SerializeObject(obj)}");
                }
            }
            return args;
        }
        private string replaceArg(string arg)
        {
            return replaceArg(new string[]{ arg });
        }
        private string replaceArg(string[] args)
        {
            string final = "";
            var lSession = this.sharedSession ? MainWindow.CurrentSession : this.session;
            foreach (string sT in args)
            {
                string s = sT;
                if (sT.Contains(" ")) s = $"\"{sT}\"";

                // GAME
                string temp = s
//                    .Replace(@"${auth_player_name}", lSession.Username)
                    .Replace(@"${version_name}", this.id)
                    .Replace(@"${game_directory}", Path.GetFullPath($"{Config.i.minecraftPath}/{this.name}"))
                    .Replace(@"${assets_root}", this.sharedAssets ? Path.GetFullPath($"{Config.i.minecraftPath}/assets") : Path.GetFullPath(this.assetsPath))
                    .Replace(@"${assets_index_name}", this.assetIndex.id)
//                    .Replace(@"${auth_uuid}", lSession.UUID)
//                    .Replace(@"${auth_access_token}", lSession.AccessToken)
                    .Replace(@"${user_type}", "microsoft") // Temporary hardcoded, sorry!
                    .Replace(@"${version_type}", this.type)

                // JVM
                    .Replace(@"${launcher_name}", Assembly.GetExecutingAssembly().GetName().Name)
                    .Replace(@"${launcher_version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // ADDITIONAL
                if(resolution != null)
                {
                    temp = temp
                        .Replace(@"${resolution_width}", this.resolution.width.ToString())
                        .Replace(@"${resolution_height}", this.resolution.height.ToString());
                }
                final += $"{temp} ";
            }
            return final;
        }

    }
}
