using MojangAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                if(features.is_demo_user != null && features.is_demo_user == true) // IS DEMO
                {
                }

                if (p.resolution != null)
                    res = true;
            }

            if (os != null)
                if(os.name != MainWindow.GetLauncherOSName()) // OS
                    res = false;
            Console.WriteLine("Allowed: " + ((this.action == "allow") ? res : !res) + "\n" + JsonConvert.SerializeObject(this));

            return (this.action == "allow") ? res : !res;
        }
    }
}
