using System;
using System.Xml;
using System.Collections.Generic;

namespace FirefoxBet365Placer.Json
{
    public class PlacedBet
    {
        public string betType { get; set; }

        public string stake { get; set; }
        public string possibleWin { get; set; }
        public string betId { get; set; }
        public string selection { get; set; }

        public List<PlacedBetRunner> runners;

        public PlacedBet(string type, string stake, string selection)
        {
            this.betType = type;
            this.stake = stake;
            this.selection = selection;
            this.runners = new List<PlacedBetRunner>();
        }

        public PlacedBet()
        {
            this.runners = new List<PlacedBetRunner>();
        }

        public string ToXML()
        {
            string ret = "<PlacedBet>";
            ret += string.Format("<BetType>{0}</BetType>", betType);
            ret += string.Format("<Stake>{0}</Stake>", stake);
            ret += string.Format("<BetId>{0}</BetId>", betId);
            ret += string.Format("<Selection>{0}</Selection>", selection);
            ret += "<Runners>";
            foreach (var runner in this.runners)
            {
                ret += runner.ToXML();
            }
            ret += "</Runners>";
            ret += "</PlacedBet>";
            return ret;
        }

        public bool parseFromXML(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode rootNode = doc.DocumentElement.SelectSingleNode("/PlacedBet");
                if (rootNode == null) return false;
                this.betType = rootNode.SelectSingleNode("BetType").InnerText;
                this.stake = rootNode.SelectSingleNode("Stake").InnerText;
                this.selection = rootNode.SelectSingleNode("Selection").InnerText;
                this.betId = rootNode.SelectSingleNode("BetId").InnerText;
                XmlNodeList nodes = rootNode.SelectNodes("Runners/Runner");
                foreach (XmlNode node in nodes)
                {
                    PlacedBetRunner newItem = new PlacedBetRunner();
                    newItem.parseFromXML("<Runner>" + node.InnerXml + "</Runner>");
                    this.runners.Add(newItem);
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool compareWith(PlacedBet B)
        {
            if (this.betType != B.betType) return false;
            else if (this.stake != B.stake) return false;

            for (int i = 0; i < this.runners.Count; i++)
            {
                PlacedBetRunner runner = this.runners[i];
                PlacedBetRunner runnerB = B.runners[i];
                if (runner.selection != runnerB.selection) return false;
                else if (runner.type != runnerB.type) return false;
                else if (runner.eventName != runnerB.eventName) return false;
            }
            return true;
        }
    }
}
