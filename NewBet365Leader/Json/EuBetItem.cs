using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class EuBetItem
    {

        public string dbId { get; set; }

        public string runnerId { get; set; }
        public string bs { get; set; }
        public string oldBS { get; set; }
        public string sport { get; set; }
        public string stake { get; set; }
        public double numStake { get; set; }
        public double odds { get; set; }
        public string handicap;
        public double oddsDistance { get; set; }
        public string tipster { get; set; }
        public string pick { get; set; }
        public List<EuAccount> userList { get; set; }
        public bool isLive { get; set; }
        public string match { get; set; }
        public string userTerm { get; set; }
        public double eventDistance { get; set; }
        public bool isValuebet { get; set; }
        public bool bDouble { get; set; }
        public bool bGradualStake { get; set; }
        public int botCount { get; set; }
        public int botIndex { get; set; }
        public string directLink { get; set; }

        public EuBetItem()
        {
            bGradualStake = false;
        }
    }
}
