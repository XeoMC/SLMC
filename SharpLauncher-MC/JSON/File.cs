using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Task DownloadTask(string path, bool redownload = true)
        {
            path = Path.GetFullPath(path);
            if (System.IO.File.Exists(path))
            {
                if (!redownload) return null;
                if (!String.IsNullOrEmpty(this.sha1))
                {
//                    if(System.IO.File.ReadAllBytes(path).GetHashSHA1() == this.sha1) return null;
                } // check filesize
            }

            Task t = new Task(async () => {
//                await MainWindow.WebClient.DownloadFileTaskAsync(new Uri(url), path);
            });
            t.Start();
            return t;
        }
    }
}
