using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class SoccerHistory
    {
        public int id { get; set; }
        public string league { get; set; }
        public string match { get; set; }
        public string pick { get; set; }
        public string handicap { get; set; }
        public double odds { get; set; }
        public DateTime placed { get; set; }
        public double dprofit { get; set; }
        public double pin_odds { get; set; }
        public double pin_reverseodds { get; set; }
        public string h_score { get; set; }
        public string a_score { get; set; }
        public string mins { get; set; }
        public int result { get; set; }
        public string bet_id { get; set; }
        public string sharpbookie { get; set; }
    }
}
