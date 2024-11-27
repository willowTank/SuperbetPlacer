using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Constants
{
    public enum State
    {
        Init,
        Running,
        Pause,
        Stop
    }


    public enum ValidationState
    {
        WITHOUT_KEY,
        SUCCESS,
        FAILURE,
    }
    public enum SOURCE
    {
        TIPSTER,
        COPYBET,
        BETBURGER,
        DOMBETTING,
        BFLIVE,
        BASHING,
        TRADEMATE,
        USAHORSE,
        DOG_DOG,
        AUS_HH,
        DOGWIN,
        RACING_INVEST,
        DOG_PREMATCH,
    }

    public enum SOLUTION 
    {
        INCOGNITION,
        VMLOGIN,
        MULTILOGIN,
        GOLOGIN
    }
    public class GlobalConstants
    {
        public static State state = State.Init;
        public static ValidationState validationState = ValidationState.WITHOUT_KEY ;

        public static string DELIM_RECORD = "\u0001";
        public static string DELIM_FIELD = "\u0002";
        public static string DELIM_HANDSHAKE_MSG = "\u0003";
        public static string DELIM_MSG = "\u0008";
        public static char CLIENT_CONNECT = (char)0;
        public static char CLIENT_POLL = (char)1;
        public static char CLIENT_SEND = (char)2;
        public static char CLIENT_CONNECT_FAST = (char)3;
        public static char INITIAL_TOPIC_LOAD = (char)20;
        public static char DELTA = (char)21;
        public static char CLIENT_SUBSCRIBE = (char)22;
        public static char CLIENT_UNSUBSCRIBE = (char)23;
        public static char CLIENT_SWAP_SUBSCRIPTIONS = (char)26;
        public static char NONE_ENCODING = (char)0;
        public static char ENCRYPTED_ENCODING = (char)17;
        public static char COMPRESSED_ENCODING = (char)18;
        public static char BASE64_ENCODING = (char)19;
        public static char SERVER_PING = (char)24;
        public static char CLIENT_PING = (char)25;
        public static char CLIENT_ABORT = (char)28;
        public static char CLIENT_CLOSE = (char)29;
        public static char ACK_ITL = (char)30;
        public static char ACK_DELTA = (char)31;
        public static char ACK_RESPONSE = (char)32;
        public static char HANDSHAKE_PROTOCOL = (char)35;//'#';
        public static char HANDSHAKE_VERSION = (char)3;
        public static char HANDSHAKE_CONNECTION_TYPE = (char)80;//'P';
        public static char HANDSHAKE_CAPABILITIES_FLAG = (char)1;
        public static string HANDSHAKE_STATUS_CONNECTED = "100";
        public static string HANDSHAKE_STATUS_REJECTED = "111";
    }


    public static class Constants
    {
        public static string googleKey = "";

        public static string[] DisabledBookies = { "betfair", "matchbook", "sbobet", "smarkets", "pinnacle", "dafabetsports", "dafabet" };

        public static string formatString = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"";
        public static string headerString = "\"Time\", \"Started\",\"%\",\"ROI\",\"Bookie\",\"Sport\",\"Event\",\"Outcome\",\"Odds\"";

        public static string formatStringBet = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"";
        public static string headerStringBet = "\"Time\",\"Sport\",\"Event\",\"Arb(%)\",\"Outcome\",\"Odds\",\"League\",\"Sharp Bookie\",\"Stake\",\"Event Date\",\"Bet Ref\",\"Result\"";

        public static string formatStringBestOddsBet = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"";
        public static string headerStringBestOddsBet = "\"TXN Time\",\"Event Time\",\"Margin(%)\",\"Profit\",\"Bookie\",\"Event\",\"League\",\"Outcome\",\"Odds\",\"Stake\",\"MaxStake\",\"Source\",\"Placed\",\"Status\"";

        public static string formatStringBestLog = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"";
        public static string headerStringBestLog = "\"Time placed\",\"Time Event\",\"Event\",\"League\",\"Selection\",\"Odds Log\",\"Placed Log\"";

        public static string formatLiveArbInfoString = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"";
        public static string headerLiveArbInfoString = "\"Time\", \"Started\",\"%\",\"ROI\",\"Bookie\",\"Sport\",\"Event\",\"Score\",\"Outcome\",\"Odds\"";

        public static string formatLiveBetString = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"";
        public static string headerLiveBetString = "\"Time\",\"%\",\"Bookie\",\"Sport\",\"Event\",\"Event Time\",\"Score\",\"Outcome\",\"Margin(%)\",\"Odds\",\"Stake\",\"Success\"";

        public static string csvFile { get; set; }
        public static string csvFileBet { get; set; }
        public static string csvFileBestLog { get; set; }

        public static string[] winUserAgents = new string[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
        };

        public static string[] macUserAgents = new string[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
        };

        public static string[] screenResolutions = new string[]
        {
            "1280x800",
            "1280x960",
            "1280x1024",
            "1360x768",
            "1400x1050",
            "1440x900",
            "1536x864",
            "1600x900",
            "1600x1200",
            "1680x1050",
            "1920x1080",
        };

    }
}
