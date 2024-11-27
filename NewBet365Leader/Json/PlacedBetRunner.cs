using System;
using System.Xml;

namespace FirefoxBet365Placer.Json
{
    public class PlacedBetRunner
    {
        public string selection { get; set; }
        public string eventName { get; set; }
        public string type { get; set; }

        public PlacedBetRunner()
        {

        }

        public PlacedBetRunner(string selection, string eventName, string type)
        {
            this.selection = selection;
            this.eventName = eventName;
            this.type = type;
        }

        public string ToXML()
        {
            string ret = "<Runner>";
            ret += string.Format("<Runner-Selection>{0}</Runner-Selection>", selection);
            ret += string.Format("<Runner-Event>{0}</Runner-Event>", eventName);
            ret += string.Format("<Runner-Type>{0}</Runner-Type>", type);
            ret += "</Runner>";
            return ret;
        }

        public bool parseFromXML(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode rootNode = doc.DocumentElement.SelectSingleNode("/Runner");
                if (rootNode == null) return false;
                this.selection = rootNode.SelectSingleNode("Runner-Selection").InnerText;
                this.eventName = rootNode.SelectSingleNode("Runner-Event").InnerText;
                this.type = rootNode.SelectSingleNode("Runner-Type").InnerText;
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
