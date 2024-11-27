using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
	[Serializable]
	public class BetData
	{
		public double dStake;
		public double dOdds;
		public int nTimeType;       // 0- Full Half 1- Half Time
		public int nType;           // 0- Goal Handicap 1- Goal OU 2- Corner Handicap 3- Corner OU
		public string strHandicap;  // Handicap or GoalLine Value like : -0.5
		public int nSide;           // 0- Home or Over, 1-Away or Under
		public string eventId;
		public string sectionId;
		public string siteCode;
		public string eventName;
		public double dReverseOdds;
		public bool isManual;
		public string strMarketName { get { return $"{(nTimeType == 0 ? "FT" : "HT")}-{(nType == 0 ? (nSide == 0 ? "AH1" : "AH2") : (nType == 1 ? (nSide == 0 ? "TO" : "TU") : (nType == 2 ? (nSide == 0 ? "CAH1" : "CAH2") : (nSide == 0 ? "CTO" : "CTU"))))}({getCorrectLine()})"; } }
		public BetData() { }

		public string getCorrectLine()
		{
			if (nSide == 0) return strHandicap;
			string ret = string.Empty;
			if (nType % 2 == 1 || strHandicap == "0")
				return strHandicap;
			ret = $"{Utils.ParseToDouble(strHandicap) * -1}";
			return ret;
		}

	}

	[Serializable]
	public class PlaceBetCandidate
	{
		public string league;
		public string home;
		public string away;
		public bool hasProfit;
		public double dProfit;
		public string strMasterCode;
		public string strSlaveCode;
		public BetData betMaster;
		public BetData betSlave;
		public bool masterPlaced;
		public bool slavePlaced;
		public DateTime masterPlaceTime;
		public DateTime slavePlaceTime;
		public string masterPlaceId;
		public string slavePlaceId;
		public string strTime;
		public string strScore;

	}
}
