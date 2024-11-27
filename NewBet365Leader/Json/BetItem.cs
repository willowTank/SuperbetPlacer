using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirefoxBet365Placer.Constants;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class BetItem
    {
        public string branch { get; set; }
        public string betId { get; set; }
        public string PD { get; set; }
        public string PD1 { get; set; }
        public string dbId { get; set; }
        public string sport { get; set; }
        public string league { get; set; }
        public string match { get; set; }
        public string outcome { get; set; }
        public string pick { get; set; }
        public string bs { get; set; }
        public double minOdds { get; set; }
        public double odds { get; set; }
        public double value { get; set; }
        public double winA { get; set; }
        public double winB { get; set; }
        public double stake { get; set; }
        public string maxStake { get; set; }
        public string runnerId { get; set; }
        public double arbPercent { get; set; }
        public string bookie { get; set; }
        public string softbookie { get; set; }
        public string eventUrl { get; set; }
        public string userTerm { get; set; }
        public double oddsDistance { get; set; }
        public double eventDistance { get; set; }
        public bool isLive { get; set; }
        public bool bEachway { get; set; }
        public bool isValuebet { get; set; }
        public bool isOddsChanged { get; set; }
        public bool isComboPlaced { get; set; }
        public bool isSinglePlaced { get; set; }
        public string eventDate { get; set; }
        public DateTime placedDate { get; set; }
        public string Placed{ get 
            {
                return placedDate.ToString("yyyy-MM-dd HH:mm:ss");
            } 
        }
        public List<string> runnerIdList { get; set; }
        public int placedCount { get; set; }
        public double totalMatched { get; set; }
        public double overround { get; set; }
        public double leftSeconds { get; set; }
        public SOURCE source { get; set; }
        public string Leader { get; set; }
        public string tipster { get; set; }
        public bool isDouble { get; internal set; }
        public bool bEW { get; internal set; }
        public int selectionCount { get; set; }
        public bool bGradualStake { get; set; }
        public string ms { get; internal set; }
        public DateTime timestamp { get; set; }
        public int retryCount { get; internal set; }
        public double newOdds { get; set; }
        public bool bSuspended { get; set; }
        public string bfMarketId { get; set; }
        public double beforeKickOff { get; set; }
        public double bf_lay { get; set; }
        public double bf_back { get; set; }
        public int no { get; set; }
        public string handicap { get; set; }
        public double pin_odds { get; set; }
        public double dprofit { get; set; }
        public string strScore { get; set; }
        public string strTime { get; set; }
        public string country { get; set; }
        public double bf_lay_a { get; set; }
        public double bf_lay_b { get; set; }
        public double bf_back_a { get; set; }
        public double bf_back_b { get; set; }
        public double b3_a { get; set; }
        public double b3_b { get; set; }
        public string FI { get; set; }
        public double pin_reverseodds { get; set; }
        public int sport_id { get; set; }
        public double line { get; set; }

        public BetItem()
        {
            placedCount = 0;
            runnerIdList = new List<string>();
            bSuspended = false;
            retryCount = 0;
            PD = string.Empty;
        }
    }
}
