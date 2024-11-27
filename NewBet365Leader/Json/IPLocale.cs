using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    public class IPLocale
    {
        public string country { get; set; }
        public string country_code { get; set; }
        public string locale { get; set; }
        public string timezone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public IPLocale(string _country, string _country_code, string _locale, string _timezone, string _latitude, string _longitude)
        {
            country = _country;
            country_code = _country_code;
            locale = _locale;
            timezone = _timezone;
            latitude = _latitude;
            longitude = _longitude;
        }
    }
}
