using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class EuAccount
    {
        public string _id { get; set; }
        public string b365Username { get; set; }
        public string b365Password { get; set; }
        public bool isBet365 { get; set; }

        public string sboUsername { get; set; }
        public string sboPassword { get; set; }
        public bool isSbo { get; set; }

        public string bfUsername { get; set; }
        public string bfPassword { get; set; }
        public bool isBf { get; set; }
        public bool isBlocked { get; set; }

        public string b365Email { get; set; }
        public string unitStake { get; set; }
        public bool isFlatStake { get; set; }
        public bool is20Stake { get; set; }
        public double placedStake { get; set; }
        public bool isLimited { get; set; }

        public string proxy { get; set; }

        public bool useLuminati { get; set; }
        public string proxyLocation { get; set; }
        public string userTerm { get; set; }

        public string proxy_user { get; set; }
        public string proxy_pass { get; set; }
        public string profileId { get; internal set; }
        public string profileName { get; internal set; }

        public EuAccount()
        {
            b365Username = string.Empty;
            b365Password = string.Empty;
            proxy = string.Empty;
            isLimited = false;
            isFlatStake = true;
            proxyLocation = "default";
            b365Email = string.Empty;
            useLuminati = true;
        }

        public EuAccount(string _username, string _password, string _proxy)
        {
            b365Username = _username;
            b365Password = _password;
            proxy = _proxy;
            isLimited = false;
            isFlatStake = true;
        }
    }
}
