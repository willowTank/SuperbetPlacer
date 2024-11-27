using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class Account
    {
        public string _id { get; set; }
        public string b365Username { get; set; }
        public string b365Password { get; set; }
        public string b365Email { get; set; }
        public string proxy { get; set; }
        public string unitStake { get; set; }
        public bool isFlatStake { get; set; }
        public bool isLimited { get; set; }

        public Account()
        {
            b365Username = string.Empty;
            b365Password = string.Empty;
            proxy = string.Empty;
            isLimited = false;
        }

        public Account(string _username, string _password)
        {
            b365Username = _username;
            b365Password = _password;
            isLimited = false;
        }
    }
}
