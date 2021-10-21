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
            var assetsPath = this.profileAssets ? $"{Config.i.minecraftPath}/profiles/{this.name}/assets" : $"{Config.i.minecraftPath}/assets";
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
                Console.WriteLine($"{GetJavaExecutable()}{args}");
                MessageBox.Show($"{GetJavaExecutable()}\n{args.Replace(" -", "\n-").Replace(";", ";\n        ")}");
                LibraryTasks.ForEach(t => t.Dispose());
                LibraryTasks.Clear();
//                MainWindow.StartWithLogger(GetJavaExecutable().Replace("\"", ""), args, "");
            }).Start();
        }
        public string GetJavaExecutable()
        {
            string t = "";
            if (this.javaPath == "")
                t += $"{Config.i.javaPath}";
            else
                t += $"{javaPath}";
            t += "/javaw.exe";

            return replaceArg(Path.GetFullPath(t));
        }
        public string GetArguments()
        {
            string args = toArgs(arguments.jvm);
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
            classpath += $"{Path.GetFullPath($"{Config.i.minecraftPath}/versions/{this.id}/{this.id}.jar")} ";
            LibraryTasks.Add(this.downloads.client.DownloadTask($"{Config.i.minecraftPath}/versions/{this.id}/{this.id}.jar"));
            var nativesPath = Path.GetFullPath($"{Config.i.minecraftPath}/natives/{this.id}");
            args = args.Replace("${classpath}", classpath).Replace("${natives_directory}", nativesPath);
            ExtractNatives(natives, nativesPath);

            if (this.addSharedJavaArgs)
                args += $"{Config.i.javaArgs} ";

            args += $"{javaArgs} ";
            args += this.logging.client.argument.Replace("${path}", $"{Path.GetFullPath($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}")} {this.mainClass} ");
            this.logging.client.file.DownloadTask($"{Config.i.minecraftPath}/assets/log_configs/{this.logging.client.file.id}");
            args += toArgs(arguments.game);
            new DirectoryInfo(Path.GetFullPath($"{Config.i.minecraftPath}/profiles/{this.name}")).Create();
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
                    Condition condition = (obj as JObject).ToObject<Condition>();
                    if (condition != null)
                        if (condition.Result(this))
                            if (condition.value is JArray conditionArray)
                                args += replaceArg(conditionArray.ToObject<string[]>());
                            else
                            {
                                if (condition.value is string conditionString)
                                    args += replaceArg(conditionString);
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
                    .Replace(@"${game_directory}", Path.GetFullPath($"{Config.i.minecraftPath}/profiles/{this.name}"))
                    .Replace(@"${assets_root}", this.profileAssets ? Path.GetFullPath($"{Config.i.minecraftPath}/profiles/{this.name}/assets") : Path.GetFullPath($"{Config.i.minecraftPath}/assets"))
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
                final += $"{s} ";
            }
            return final;
        }

    }
}
