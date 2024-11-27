using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    [Serializable]
    public class RaceItem
    {
        public string trackTitle { get; set; }
        public string raceTitle { get; set; }
        public DateTime raceStart { get; set; }
        public string directLink { get; set; }

        public string winMarketId { get; set; }
        public string placeMarketId { get; set; }
        public string raceId { get; set; }

        public List<HorseItem> horseList { get; set; }

        public string GetJsonContent()
        {
            dynamic content = new JObject();
            content.raceTitle = raceTitle;
            content.horses = new JArray();
            if (horseList != null)
            {
                foreach (HorseItem item in horseList)
                {
                    content.horses.Add(item.match);
                }
            }
            return content.ToString();
        }

        public bool CheckCondition()
        {
            int counter = 0;
            foreach (HorseItem horseItem in horseList) 
            {
                if (horseItem.odds <= 1.9)
                {
                    return true;
                }
                else if (horseItem.odds <= 2.5)
                {
                    counter++;
                    if(counter >= 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
