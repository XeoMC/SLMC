using MojangAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public enum Feature
    {
        is_demo_user,
        has_custom_resolution
    }
    public class Rule
    {
        public string action;
        public Features features;
        public OS os;
        public bool isAllowed(Profile p)
        {
            bool res = true;

            if(features != null)
            {
                Session lSession = p.session;
                if (p.sharedSession) 
                lSession = MainWindow.CurrentSession;

                if(features.is_demo_user != true) // IS DEMO
                    res = false;

                if (p.resolution == null)
                    res = false;
            }

            if (os != null)
            {
                if(!String.IsNullOrEmpty(os.name) && os.name != MainWindow.GetLauncherOSName()) // OS
                    res = false;
                if (!String.IsNullOrEmpty(os.arch) && os.arch.ToUpper() != (RuntimeInformation.OSArchitecture.ToString() == "X64" ? "X86" : RuntimeInformation.OSArchitecture.ToString()).ToUpper()) // minecraft thinks x86 = x64
                    res = false;
                if (!String.IsNullOrEmpty(os.version) && !Regex.Match(Environment.OSVersion.Version.ToString(), os.version).Success) // not really sure how to check it
                    res = false;
            }
#if DEBUG
//            Console.WriteLine($"Allowed: {((this.action == "allow") ? res : !res) + "\n" + JsonConvert.SerializeObject(this)}");
#endif

            return (this.action == "allow") ? res : !res;
        }
    }
}
