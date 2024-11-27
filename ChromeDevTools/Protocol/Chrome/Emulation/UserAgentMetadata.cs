using MasterDevs.ChromeDevTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDevTools.Protocol.Chrome.Emulation
{
    [SupportedBy("Chrome")]
    public class UserAgentMetadata
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UserAgentBrandVersion[] Brands { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UserAgentBrandVersion[] FullVersionList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullVersion { get; set; }

        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string Architecture { get; set; }
        public string Model { get; set; }
        public bool Mobile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Bitness { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Wow64 { get; set; }

    }

    public class UserAgentBrandVersion
    {
        public string Brand { get; set; }
        public string Version { get; set; }

    }
}
