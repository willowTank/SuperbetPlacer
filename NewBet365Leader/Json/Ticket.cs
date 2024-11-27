using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Json
{
    internal class Ticket
    {
        public Ticket() { }
        public string ticketOnline { get; set; }
        public int total { get; set; }
        public string betType { get; set; }
        public string combs { get; set; }
        public string clientSourceType { get; set; }
        public int paymentBonusType { get; set; }
        public string locale { get; set; }
        public string deviceIdentifier { get; set; }
        public string autoAcceptChanges { get; set; }
        public string ticketUuid { get; set; }
        public string oddDescription { get; set; }

        public TicketBetItem[] items { get; set; }

        public RequestDetail requestDetails { get; set; }
    }
}
