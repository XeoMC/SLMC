using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public enum Action
    {
        allow,
        disallow
    }
    public enum Feature
    {
        is_demo_user,
        has_custom_resolution
    }
    public class Rule
    {
        public Action action;
        public Dictionary<Feature, bool> features;
        public OS os;
        public bool Result(Profile p)
        {
            if (p.sharedSession) { } // IS DEMO
//                MainWindow.CurrentSession;
            else { }
//                p.session
            if (p.resolution != null) // RESOLUTION
                return true;
            if (os.name == MainWindow.getLauncherOSName()) // OS
                return true;

            return false;
        }
    }
}
