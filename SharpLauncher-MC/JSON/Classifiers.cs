using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC.JSON
{
    public class Classifiers
    {
        public File javadoc;
        [JsonProperty(PropertyName = "natives-osx")]
        public File natives_osx;
        [JsonProperty(PropertyName = "natives-linux")]
        public File natives_linux;
        [JsonProperty(PropertyName = "natives-windows")]
        public File natives_windows;
        public File sources;
    }
}
