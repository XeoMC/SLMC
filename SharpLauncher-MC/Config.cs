using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpLauncher_MC
{
    public class Config
    {
        public static Config i;
        public bool LargeMode = false;
        public Size resolution = new Size(280,420);
        public double settingsSize = 1.5;
    }
}
