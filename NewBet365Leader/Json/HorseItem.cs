using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class HorseItem
    {
        public int no { get; set; }
        public string league { get; set; }
        public double arbPercent { get; set; }
        public string match { get; set; }
        public double odds { get; set; }
        public string bs { get; set; }
        public string runnerId { get; set; }
        public string directLink { get; set; }
        public long foundTime { get; set; }

        public JObject getJsonInfo()
        {
            dynamic newTipData = new JObject();
            newTipData.tipster = "EWS";
            newTipData.league = this.league;
            newTipData.match = this.match;
            newTipData.bs = this.bs;
            newTipData.odds = this.odds;
            newTipData.runnerId = this.runnerId;
            newTipData.directLink = this.directLink;
            newTipData.arbPercent = this.arbPercent;
            newTipData.foundTime = this.foundTime;
            newTipData.isValuebet = true;
            return newTipData;
        }

        public JObject getMyJsonInfo()
        {
            dynamic newTipData = new JObject();
            newTipData.tipster = "BookieBashing";
            newTipData.League = this.league;
            newTipData.Runner = this.match;
            newTipData.Pick = "WIN";
            newTipData.Odds = this.odds;
            newTipData.DiffOdds = this.arbPercent;
            newTipData.RunnerId = this.runnerId;
            newTipData.bs = this.bs;
            return newTipData;
        }
    }
}
