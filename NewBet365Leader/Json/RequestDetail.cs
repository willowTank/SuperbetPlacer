using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    internal class RequestDetail
    {
        public RequestDetail() { }
        public string ldAnonymousUserKey { get; set; }
        public string deviceId { get; set; }
        public string isDeviceIdTestFlagOnSubscribed { get; set; }
        public string isDeviceIdTestFlagOnInitial {  get; set; }
    }
}
