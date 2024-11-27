using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class BrowserProfile
    {
        public int id { get; set; }
        public string platform { get; set; }
        public int hardwareConcurrency { get; set; }
        public int maxTouchPoints { get; set; }
        public string userAgent { get; set; }
        public string appVersion { get; set; }

        public void PrintMe()
        {
            
        }

        public string GetAppVersion()
        {
            return userAgent.Replace("Mozilla/", "");
        }
    }
}
