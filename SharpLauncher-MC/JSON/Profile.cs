using MojangAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpLauncher_MC.JSON.ClassicLauncher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpLauncher_MC.JSON
{
    public class Profile : ClassicVersion
    {
        // SLMC Profile
        public string name = "Example";
        public string icon = "Grass";
        public bool profileAssets;
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
        public void DownloadAssets()
        {
            var assetsPath = this.profileAssets ? $"{GetProfilePath()}/assets" : $"{Config.i.minecraftPath}/assets";
            var assets = JsonConvert.DeserializeObject<AssetIndex>(assetIndex.DownloadString());
            foreach(KeyValuePair<string, AssetIndexObject> kvp in assets.objects)
            {
                string filePath = Path.GetFullPath($"{assetsPath}/{kvp.Value.hash.Substring(0, 2)}/{kvp.Value.hash}");
                if (System.IO.File.Exists(filePath)) continue; // temp, need to check hash
                using (var client = new WebClient())
                {
                    new FileInfo(filePath).Directory.Create();
                    client.DownloadFile(new Uri($"http://resources.download.minecraft.net/{kvp.Value.hash.Substring(0, 2)}/{kvp.Value.hash}"), filePath);
#if DEBUG
                    Console.WriteLine(filePath + " downloaded");
#endif
                }
            }
        }
        public async void ExtractNatives(List<Library> libraries, string nativesPath)
        {
            foreach(Library l in libraries)
            {
                var osN = "natives-" + MainWindow.GetLauncherOSName().Replace("osx", "macos");
                if (l.natives.windows == osN)
                {
                    var saveLocation = $"{Config.i.minecraftPath}/libraries/{l.downloads.classifiers.natives_windows.path}";
                    Task dlTask = l.downloads.classifiers.natives_windows.DownloadTask(saveLocation);
                    LibraryTasks.Add(dlTask);
                    await Task.WhenAll(dlTask); // now extract
                    Task t = new Task(() =>
                    {
                        Console.WriteLine($"Extracting {l.downloads.classifiers.natives_windows.path}");
                        if( String.IsNullOrEmpty(Config.i.externalUnpacker))
                        {
                            throw new NotImplementedException("Please, use external unpacker parameter for now.");
                        } else
                        {
                            var process = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = Path.GetFullPath(Config.i.externalUnpacker),
                                    Arguments = Config.i.externalUnpackerArgs
                                        .Replace(@"${file}", saveLocation)
                                        .Replace(@"${path}", nativesPath)
                                }
                            };
#if DEBUG
                            process.OutputDataReceived += (s, e) =>
                            {
                                Console.WriteLine("UNPACKER: " + e.Data);
                            };
#endif
                            new DirectoryInfo(nativesPath).Create();
                            Console.WriteLine($"Started unpacker: {process.Start()}");
                            process.WaitForExit();
                        }
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
        public void Start(bool logger = true)
        {
            new Task(async () =>
            {
                var args = GetArguments();
                DownloadAssets();
                await Task.WhenAll(LibraryTasks);
                Console.WriteLine($"\"{GetJavaExecutable()}\" {String.Join(" ", args)}");
                await Task.Delay(250); // strange crash after all tasks are finished, maybe it'll fix stuff
                LibraryTasks.ForEach(t => t.Dispose());
                LibraryTasks.Clear();
                MainWindow.StartWithLogger(GetJavaExecutable(), String.Join("~~~", args), GetProfilePath());
            }).Start();
        }
        public string GetJavaExecutable() => Path.GetFullPath($"{(this.javaPath == "" ? Config.i.javaPath : javaPath)}/javaw.exe");
        public string GetProfilePath() => Path.GetFullPath($"{Config.i.minecraftPath}/profiles/{this.name}");
        public string GetAssetsPath() => Path.GetFullPath(this.profileAssets ? $"{GetProfilePath()}/assets" : $"{Config.i.minecraftPath}/assets");
        public List<string> GetArguments()
        {
            List<string> args = toArgs(arguments.jvm);
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
                        LibraryTasks.Add(l.downloads.artifact.DownloadTask(path));
                        classpath += $"{path};";
                    }
                    if(l.natives != null)
                        natives.Add(l);
                }
            }
            classpath += $"{Path.GetFullPath($"{Config.i.minecraftPath}/natives/{this.id}/{this.id}.jar")} ";
            LibraryTasks.Add(this.downloads.client.DownloadTask($"{Config.i.minecraftPath}/versions/{this.id}/{this.id}.jar"));
            var nativesPath = Path.GetFullPath($"{Config.i.minecraftPath}/natives/{this.id}");
            for(int i = 0; i < args.Count; i++) { args[i] = args[i].Replace("${classpath}", classpath).Replace("${natives_directory}", nativesPath); }
            ExtractNatives(natives, nativesPath);

            if (this.addSharedJavaArgs && Config.i.javaArgs.Length != 0)
                args.AddRange(Config.i.javaArgs.Split(' '));
            if(javaArgs.Length != 0)
                args.AddRange(javaArgs.Split(' '));
            args.Add(this.logging.client.argument.Replace("${path}", $"{Path.GetFullPath($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}")} {this.mainClass}"));
            this.logging.client.file.DownloadTask($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}");
            args.AddRange(toArgs(arguments.game));
            new DirectoryInfo(GetProfilePath()).Create();
            return args;
        }

        private List<string> toArgs(object[] list)
        {
            List<string> args = new List<string>();
            foreach (object obj in list)
            {
                if (obj is string)
                {
                    args.AddRange(replaceArg((string)obj));
                }
                else
                {
                    Condition condition = (obj as JObject).ToObject<Condition>();
                    if (condition != null)
                        if (condition.Result(this))
                            if (condition.value is JArray conditionArray)
                                args.AddRange(replaceArg(conditionArray.ToObject<string[]>()));
                            else
                            {
                                if (condition.value is string conditionString)
                                    args.AddRange(replaceArg(conditionString));
                                else
                                    Console.WriteLine($"Condition value is undefined:\n{JsonConvert.SerializeObject(obj)}");
                            }
                        else
                            continue;
                    else
                        Console.WriteLine($"Argument is not a string or a condition:\n{JsonConvert.SerializeObject(obj)}");
                }
            }
            return args;
        }
        private List<string> replaceArg(string arg)
        {
            return replaceArg(new string[]{ arg });
        }
        private List<string> replaceArg(string[] args)
        {
            List<string> final = new List<string>();
            var lSession = this.sharedSession ? MainWindow.CurrentSession : this.session;
            foreach (string sT in args)
            {
                string s = sT;
                if (s.Contains(" ")) s = $"\"{s}\"";

                if (lSession != null)
                {
                    s = s
                        .Replace(@"${auth_player_name}", lSession.Username)
                        .Replace(@"${auth_uuid}", lSession.UUID)
                        .Replace(@"${auth_access_token}", lSession.AccessToken)
                        .Replace(@"${user_type}", "mojang"); // Temporary hardcoded, sorry!
                }

                // GAME
                s = s
                    .Replace(@"${version_name}", this.id)
                    .Replace(@"${game_directory}", GetProfilePath())
                    .Replace(@"${assets_root}", GetAssetsPath())
                    .Replace(@"${assets_index_name}", this.assetIndex.id)
                    .Replace(@"${version_type}", this.type)

                // JVM
                    .Replace(@"${launcher_name}", Assembly.GetExecutingAssembly().GetName().Name)
                    .Replace(@"${launcher_version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // ADDITIONAL
                if(resolution != null)
                {
                    s = s
                        .Replace(@"${resolution_width}", this.resolution.width.ToString())
                        .Replace(@"${resolution_height}", this.resolution.height.ToString());
                }
                final.Add(s);
            }
            return final;
        }

    }
}
