using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    internal class TicketBetItem
    {
        public TicketBetItem() { }
        public int oddTypeId { get; set; }
        public string sbValue { get; set; }
        public int matchId { get; set; }
        public string value { get; set; }
        public string matchName { get; set; }
        public string oddFullName { get; set; }
        public string matchDate { get; set; }
        public string matchDateUtc { get; set; }
        public int betGroupId { get; set; }
        public bool selected { get; set; }
        public string type { get; set; }
        public bool fix { get; set; }
        public string teamnameone { get; set; }
        public string teamnametwo { get; set; }
        public string tournamentName { get; set; }
        public string sportName { get; set; }
        public string teamId1 { get; set; }
        public string teamId2 { get; set; }
        public string betRadarId { get; set; }
        public int tournamentId { get; set; }
        public string oddDescription { get; set; }
        public int sportId { get; set; }
        public bool live { get; set; }
        public int eventId { get; set; }
        public string eventUuid { get; set; }
        public string eventName { get; set; }
        public int eventCode { get; set; }
        public int marketId { get; set; }
        public string marketName { get; set; }
        public string marketUuid { get; set; }
        public int oddId { get; set; }
        public string oddName { get; set; }
        public string oddCode { get; set; }
        public string uuid { get; set; }
        public string oddUuid { get; set; }
        public int sourceType { get; set; }
        public int sourceScreen { get; set; }
    }
}