using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class MatchUpInfo
    {
        public DateTime openDate { get; set; }
        public bool bClosed { get; set; }

        public string Race { get { return raceName; } }
        public double LeftSeconds
        {
            get
            {
                try
                {
                    DateTime nowTime = DateTime.UtcNow;
                    double diffSeconds = (openDate - nowTime).TotalSeconds;
                    return Math.Round(diffSeconds);
                }
                catch
                {

                }
                return 0;
            }
        }

        public double decOddsA_B
        {
            get
            {
                try
                {
                    return Utils.FractionToDouble(oddsToBeatA_B);
                }
                catch
                {

                }
                return 0;
            }
        }

        public double decOddsB_A
        {
            get
            {
                try
                {
                    return Utils.FractionToDouble(oddsToBeatB_A);
                }
                catch
                {

                }
                return 0;
            }
        }

        public double decOpenedOddsA_B
        {
            get
            {
                try
                {
                    return Utils.FractionToDouble(openedOddsToBeatA_B);
                }
                catch
                {

                }
                return 0;
            }
        }

        public double decOpenedOddsB_A
        {
            get
            {
                try
                {
                    return Utils.FractionToDouble(openedOddsToBeatB_A);
                }
                catch
                {

                }
                return 0;
            }
        }

        public string bfOdds_A
        {
            get
            {
                try
                {
                    return string.Format("{0}-{1}", backOddsOfA, layOddsOfA);
                }
                catch
                {

                }
                return "";
            }
        }

        public string bfOdds_B
        {
            get
            {
                try
                {
                    return string.Format("{0}-{1}", backOddsOfB, layOddsOfB);
                }
                catch
                {

                }
                return "";
            }
        }

        public double ValueAB
        {
            get
            {
                try
                {
                    double oddsA = oddsOfrunnerA;
                    double oddsB = oddsOfrunnerB;

                    double PA = 1 / oddsA;
                    double PB = 1 / oddsB;
                    double PVSA = PA / (PA + PB);
                    double PVSB = PB / (PA + PB);

                    double oddsToA_B = Utils.FractionToDouble(oddsToBeatA_B);
                    double oddsToB_A = Utils.FractionToDouble(oddsToBeatB_A);

                    double diffOddsA = 100 * (PVSA * oddsToA_B - 1);
                    return Math.Round(diffOddsA, 2);
                }
                catch
                {
                }
                return 0;
            }
        }

        public double ValueBA
        {
            get
            {
                try
                {
                    double oddsA = oddsOfrunnerA;
                    double oddsB = oddsOfrunnerB;

                    double PA = 1 / oddsA;
                    double PB = 1 / oddsB;
                    double PVSA = PA / (PA + PB);
                    double PVSB = PB / (PA + PB);
                    double oddsToA_B = Utils.FractionToDouble(oddsToBeatA_B);
                    double oddsToB_A = Utils.FractionToDouble(oddsToBeatB_A);

                    double diffOddsB = 100 * (PVSB * oddsToB_A - 1);
                    return Math.Round(diffOddsB, 2);
                }
                catch
                {
                }
                return 0;
            }
        }

        public double aWinValue
        {
            get
            {
                try
                {
                    return Math.Round((b365AOdds - oddsOfrunnerA) / oddsOfrunnerA * 100);
                }
                catch
                {

                }
                return 0;
            }
        }

        public double bWinValue
        {
            get
            {
                try
                {
                    return Math.Round((b365BOdds - oddsOfrunnerB) / oddsOfrunnerB * 100);
                }
                catch
                {
                }
                return 0;
            }
        }

        public string raceName { get; set; }
        public string raceId { get; set; }
        public string marketId { get; set; }
        public string runnerAId { get; set; }
        public string runnerBId { get; set; }
        public string runnerA { get; set; }
        public string runnerB { get; set; }

        public int runnerANo { get; set; }
        public int runnerBNo { get; set; }

        public string oddsToBeatA_B { get; set; }
        public string oddsToBeatB_A { get; set; }

        public string openedOddsToBeatA_B { get; set; }
        public string openedOddsToBeatB_A { get; set; }

        public double oddsOfrunnerA { get; set; }
        public double oddsOfrunnerB { get; set; }
        public double totalMatched { get; set; }
        public double overround { get; set; }
        public double backOddsOfA { get; set; }
        public double layOddsOfA { get; set; }
        public double backOddsOfB { get; set; }
        public double layOddsOfB { get; set; }

        public double b365AOdds { get; set; }
        public double b365BOdds { get; set; }

        public bool bUpdated { get; set; }
        public bool bSuspended { get; set; }
        public string raceIId { get; internal set; }
        public string type { get; set; }

        public MatchUpInfo()
        {
            raceName = string.Empty;
            marketId = string.Empty;
            oddsOfrunnerA = 0;
            oddsOfrunnerB = 0;
            bClosed = false;
            backOddsOfA = 0;
            layOddsOfA = 0;

            backOddsOfB = 0;
            layOddsOfB = 0;

            totalMatched = 0;

            runnerA = string.Empty;
            runnerB = string.Empty;
            runnerANo = -1;
            runnerBNo = -1;
            runnerAId = string.Empty;
            runnerBId = string.Empty;
            oddsToBeatA_B = string.Empty;
            oddsToBeatB_A = string.Empty;


            bUpdated = true;
        }

        public MatchUpInfo Clone()
        {
            MatchUpInfo clonedItem = new MatchUpInfo();
            clonedItem.raceName = raceName;
            clonedItem.marketId = marketId;
            clonedItem.runnerAId = runnerAId;
            clonedItem.runnerBId = runnerBId;
            clonedItem.runnerA = runnerA;
            clonedItem.runnerB = runnerB;
            clonedItem.bClosed = bClosed;
            clonedItem.runnerANo = runnerANo;
            clonedItem.runnerBNo = runnerBNo;
            clonedItem.oddsToBeatA_B = oddsToBeatA_B;
            clonedItem.oddsToBeatB_A = oddsToBeatB_A;
            clonedItem.oddsOfrunnerA = oddsOfrunnerA;
            clonedItem.oddsOfrunnerB = oddsOfrunnerB;
            clonedItem.totalMatched = totalMatched;

            clonedItem.backOddsOfA = backOddsOfA;
            clonedItem.backOddsOfB = backOddsOfB;
            clonedItem.layOddsOfA = layOddsOfA;
            clonedItem.layOddsOfB = layOddsOfB;

            clonedItem.bUpdated = bUpdated;

            return clonedItem;
        }


        public string betBSstring(string runnerID)
        {
            string strFmt = "pt=N#o={0}#f={1}#fp={2}#so=0#c=4#sa=SA_STR#oto=1#st=#ust=#fb=0.00#tr=#||";
            string odds = oddsToBeatA_B;
            if (runnerID == runnerBId)
                odds = oddsToBeatB_A;
            return string.Format(strFmt, odds, raceId, runnerID);
        }
    }
}
