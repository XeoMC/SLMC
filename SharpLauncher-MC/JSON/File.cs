using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class File
    {
        public string id;
        public string sha1;
        public ulong size;
        public ulong totalSize;
        public string url;
        public string path;
        public string DownloadString()
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(new Uri(url));
            }
        }
        public Task DownloadTask(string path, bool verify = true)
        {
            path = Path.GetFullPath(path);
            if (System.IO.File.Exists(path))
            {
                if (!verify) return null;
                if (!String.IsNullOrEmpty(this.sha1))
                {
//                    if(System.IO.File.ReadAllBytes(path).GetHashSHA1() == this.sha1) return null;
                } // check filesize
            }

            Task t = new Task(() => {
                using (var client = new WebClient())
                {
                    new FileInfo(path).Directory.Create();
                    client.DownloadFile(new Uri(url), path);
#if DEBUG
                    Console.WriteLine((String.IsNullOrEmpty(this.path) ? this.url : this.path) + " downloaded");
#endif
                }
            });
            t.Start();
            return t;
        }
    }
}
