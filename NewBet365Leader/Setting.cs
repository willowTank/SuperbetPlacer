using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net;

namespace FirefoxBet365Placer
{
    public enum PLACING_MODE 
    {
        NORMAL,
        FAST,
        SEARCH
    }
    public class Setting
    {
        private static Setting _instance = null;

        public static Setting instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Setting();

                return _instance;
            }
        }

        public CookieContainer _CookieContainer = new CookieContainer();
        public bool isOnline { get; set; }
        public bool isRecordResult { get; set; }
        public dynamic BotSetting { get; set; }

        // General Setting
        public int RemoteDebuggingPort { get; set; }
        public string serverAddr { get; set; }
        public string proxyServer { get; set; }
        public string proxyUser { get; set; }
        public string proxyPass { get; set; }

        public string bet365Domain { get; set; }
        public string countryCode { get; set; }
        public string betUsername { get; set; }
        public string betPassword { get; set; }
        public string license { get; set; }
        public string profileId { get; set; }

        public bool isLeader { get; set; }
        public bool isKeepSessionAlive { get; set; }
        public bool isMakeFakeBet { get; set; }

        //Bookie bashing
        public bool isFlatStake { get; set; }
        public double numFlatStake { get; set; }
        public double numBetCount { get; set; }
        public double numOddsMin { get; set; }

        public int innerWidth { get; internal set; }
        public int innerHeight { get; internal set; }

        public string sessionUrl
        {
            get
            {
                return "https://www." + Setting.instance.bet365Domain + "/sessionactivityapi/setlastactiontime";
            }
        }

        public string bet365Link
        {
            get
            {
                return "https://www." + Setting.instance.bet365Domain + "/#/AS/B1/";
            }
        }

        public string ProxyUrl {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(proxyServer)) return string.Empty;
                    return proxyServer.Split(':')[0];
                }
                catch
                {
                }
                return string.Empty;
            }
        }

        public int ProxyPort {
            get 
            {
                try
                {
                    if (string.IsNullOrEmpty(proxyServer)) return 0;
                    return int.Parse(proxyServer.Split(':')[1]);
                }
                catch
                {
                }
                return 0;
            }  
        }

        public string version { get; set; }
        public bool isComplex { get; set; }
        public bool isDouble { get; set; }
        public bool isMessiBot { get; set; }
        public bool isWorkingAtDayTime { get; set; }
        public bool isClickMode { get; set; }
        public bool isEnabled3PAUS { get;  set; }
        public bool isEnabledChrome { get; internal set; }
        public bool isEnabledFirefox { get; internal set; }
        public bool isEnabledEdge { get; internal set; }
        public string chromePath { get; internal set; }
        public string firefoxPath { get; internal set; }
        public string edgePath { get; internal set; }
        public onWriteRestartLogEvent writeRestartLog { get; internal set; }
        public int solutionType { get; internal set; }

        public double delayStart_Load { get; internal set; }
        public double delayLoad_Login { get; internal set; }
        public double delayAfterLogin { get; internal set; }
        public double delayBetweenBets { get; internal set; }
        public double delayAfterRefresh { get; internal set; }
        public double delayBetweenRetries { get; internal set; }
        public double numAgentPort { get; internal set; }
        public double numStake { get; internal set; }
        public string anydesk { get; internal set; }
        public bool isUseUILogin { get; set; }
        public IntPtr WindowHandle { get; set; }
        public double heightDiff { get; set; }
        public string owner { get; set; }
        public PLACING_MODE placingMode { get; internal set; }
        public string timeStart { get; set; }
        public string timeStop { get; set; }
        public string qrAPI { get; internal set; }

        public string dolphinUrl { get; set; }

        public string dolphinToken { get; set; }

        public string dolphinProfileId { get; set; }

        public Setting()
        {
            serverAddr = "http://194.135.92.159:5002";
            numAgentPort = 8880;
            proxyServer = string.Empty;
            bet365Domain = "bet365.com";
            RemoteDebuggingPort = 25101;
            isComplex = false;
            isClickMode = false;
            innerWidth = 0;
            innerHeight = 0;
            heightDiff = 71;
        }

        public string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            if (!STR.Contains(FirstString)) return string.Empty;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            if (LastString != null)
            {
                STR = STR.Substring(Pos1);
                int Pos2 = STR.IndexOf(LastString);
                if (Pos2 > 0)
                    FinalString = STR.Substring(0, Pos2);
                else
                    FinalString = STR;
            }
            else
            {
                FinalString = STR.Substring(Pos1);
            }

            return FinalString;
        }

        public string ReplaceStr(string STR, string ReplaceSTR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            if (LastString != null)
            {
                string preSTR = STR.Substring(0, Pos1);
                int Pos2 = STR.IndexOf(LastString, Pos1);
                FinalString = preSTR + ReplaceSTR + STR.Substring(Pos2);
            }
            else
            {
                string preSTR = STR.Substring(0, Pos1);
                FinalString = preSTR + ReplaceSTR;
            }

            return FinalString;
        }

        public string ReadRegistry(string KeyName)
        {
            string usr = Setting.instance.betUsername;
            return Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + usr).GetValue(KeyName, (object)"").ToString();
        }

        public void WriteRegistry(string KeyName, string KeyValue)
        {
            try
            {
                string usr = Setting.instance.betUsername;
                Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + usr).SetValue(KeyName, (object)KeyValue);
            }
            catch
            {
            }
        }

        public string ReadCommonRegistry(string KeyName)
        {
            string usr = Setting.instance.betUsername;
            return Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-Common").GetValue(KeyName, (object)"").ToString();
        }

        public void WriteCommonRegistry(string KeyName, string KeyValue)
        {
            try
            {
                string usr = Setting.instance.betUsername;
                Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-Common").SetValue(KeyName, (object)KeyValue);
            }
            catch
            {
            }
        }

        public string ReadAccountInfo()
        {
            try
            {
                return File.ReadAllText("account.txt");
            }
            catch (Exception)
            {

            }
            return ":";
        }

        public void addCookie(Cookie newCookie, string domain)
        {
            CookieContainer newContainer = new CookieContainer();
            CookieCollection cookies = _CookieContainer.GetCookies(new Uri(domain));
            if (cookies.Count == 0)
            {
                newContainer.Add(new Uri(domain), newCookie);
            }
            else
            {
                bool bContain = false;
                foreach (Cookie cookie in cookies)
                {
                    if (newCookie.Name == cookie.Name)
                    {
                        newContainer.Add(new Uri(domain), newCookie);
                        bContain = true;
                    }
                    else
                    {
                        newContainer.Add(new Uri(domain), cookie);
                    }
                }

                if (!bContain)
                    newContainer.Add(new Uri(domain), newCookie);
            }

            _CookieContainer = newContainer;
        }

        public void WriteAccountInfo(string KeyName)
        {
            File.WriteAllText("account.txt", KeyName);
        }

        public void saveSettingInfo()
        {
            WriteAccountInfo(Setting.instance.betUsername + ":" + Setting.instance.betPassword);
            WriteCommonRegistry("timeStart", Setting.instance.timeStart);
            WriteCommonRegistry("timeStop", Setting.instance.timeStop);
            WriteRegistry("pw-version", Setting.instance.version);
            WriteRegistry("license", Setting.instance.license);
            WriteRegistry("profileId", Setting.instance.profileId);
            WriteRegistry("anydesk", Setting.instance.anydesk);
            WriteRegistry("owner", Setting.instance.owner);
            //General setting
            WriteRegistry("serverAddr", Setting.instance.serverAddr);
            WriteRegistry("proxyServer", Setting.instance.proxyServer);
            WriteRegistry("qrAPI", Setting.instance.qrAPI);
            WriteRegistry("countryCode", Setting.instance.countryCode);
            WriteRegistry("numStake", Setting.instance.numStake.ToString());
            WriteRegistry("isMakeFakeBet", Setting.instance.isMakeFakeBet ? "true" : "false");
            WriteRegistry("isDouble", Setting.instance.isDouble ? "true" : "false");
            WriteRegistry("isMessiBot", Setting.instance.isMessiBot ? "true" : "false");
            WriteRegistry("isComplex", Setting.instance.isComplex ? "true" : "false");
            WriteRegistry("isLeader", Setting.instance.isLeader ? "true" : "false");
            WriteRegistry("isWorkingAtDayTime", Setting.instance.isWorkingAtDayTime ? "true" : "false");
            WriteRegistry("isResult", Setting.instance.isKeepSessionAlive ? "true" : "false");
            // Betburger
            //Bookie Bashing
            WriteRegistry("isEnabled3PAUS", Setting.instance.isEnabled3PAUS ? "true" : "false");
            WriteRegistry("isEnabledH2H", Setting.instance.isRecordResult ? "true" : "false");
            WriteRegistry("isFlatStake", Setting.instance.isFlatStake ? "true" : "false");
            WriteRegistry("numBetCount", Setting.instance.numBetCount.ToString());
            WriteRegistry("numOddsMin", Setting.instance.numOddsMin.ToString());
            WriteRegistry("numFlatStake", Setting.instance.numFlatStake.ToString());
            WriteRegistry("heightDiff", Setting.instance.heightDiff.ToString());

            WriteRegistry("isEnabledFirefox", Setting.instance.isEnabledFirefox ? "true" : "false");
            WriteRegistry("isEnabledChrome", Setting.instance.isEnabledChrome ? "true" : "false");
            WriteRegistry("isEnabledEdge", Setting.instance.isEnabledEdge ? "true" : "false");

            WriteRegistry("chromePath", Setting.instance.chromePath);
            WriteRegistry("firefoxPath", Setting.instance.firefoxPath);
            WriteRegistry("edgePath", Setting.instance.edgePath);

            WriteRegistry("solutionType", Setting.instance.solutionType.ToString());

            //Delay
            WriteRegistry("isUseUILogin", Setting.instance.isUseUILogin ? "true" : "false");

            WriteRegistry("delayAfterLogin", Setting.instance.delayAfterLogin.ToString());
            WriteRegistry("delayAfterRefresh", Setting.instance.delayAfterRefresh.ToString());
            WriteRegistry("delayBetweenRetries", Setting.instance.delayBetweenRetries.ToString());
            WriteRegistry("delayLoad_Login", Setting.instance.delayLoad_Login.ToString());
            WriteRegistry("delayStart_Load", Setting.instance.delayStart_Load.ToString());
            WriteRegistry("delayBetweenBets", Setting.instance.delayBetweenBets.ToString());
            WriteRegistry("placingMode", ((int)Setting.instance.placingMode).ToString());

            //Dolphin
            WriteRegistry("dolphinUrl", Setting.instance.dolphinUrl.ToString());
            WriteRegistry("dolphinToken", Setting.instance.dolphinToken.ToString());
            WriteRegistry("dolphinProfileId", Setting.instance.dolphinProfileId.ToString());
        }

        public void loadSettingInfo()
        {
            string accountInfo = ReadAccountInfo();
            string[] tempArr = accountInfo.Split(':');
            if (tempArr.Length == 2)
            {
                Setting.instance.betUsername = tempArr[0];
                Setting.instance.betPassword = tempArr[1];
            }
            else { }

            // General
            Setting.instance.timeStart = ReadCommonRegistry("timeStart");
            if (string.IsNullOrEmpty(Setting.instance.timeStart)) Setting.instance.timeStart = "00:00";

            Setting.instance.timeStop = ReadCommonRegistry("timeStop");
            if (string.IsNullOrEmpty(Setting.instance.timeStop)) Setting.instance.timeStop = "11:00";

            Setting.instance.serverAddr = ReadRegistry("serverAddr");
            if (string.IsNullOrEmpty(Setting.instance.serverAddr)) Setting.instance.serverAddr = "http://37.187.91.64:5002";
            Setting.instance.version = ReadRegistry("pw-version");
            if (string.IsNullOrEmpty(Setting.instance.version)) Setting.instance.version = "1.0";
            //Setting.instance.version = "1.84";

            Setting.instance.anydesk = ReadRegistry("anydesk");
            Setting.instance.owner = ReadRegistry("owner");
            Setting.instance.qrAPI = ReadRegistry("qrAPI");
            Setting.instance.proxyServer = ReadRegistry("proxyServer");

            Setting.instance.countryCode = ReadRegistry("countryCode");
            if (string.IsNullOrEmpty(Setting.instance.countryCode)) Setting.instance.countryCode = "es";

            Setting.instance.profileId = ReadRegistry("profileId");
            Setting.instance.license   = ReadRegistry("license");
            
            Setting.instance.isComplex = readBoolKey("isComplex", false);
            Setting.instance.isDouble = readBoolKey("isDouble", false);

            Setting.instance.isLeader = readBoolKey("isLeader");
            Setting.instance.isKeepSessionAlive = readBoolKey("isResult");
            Setting.instance.numStake = readDoubleKey("numStake", 1);

            // Betburger
            Setting.instance.isWorkingAtDayTime = readBoolKey("isWorkingAtDayTime", false);
            Setting.instance.isMakeFakeBet = readBoolKey("isMakeFakeBet", false);
            Setting.instance.isMessiBot = readBoolKey("isMessiBot", false);
            // Bookie Bashing
            Setting.instance.solutionType = (int)readDoubleKey("solutionType", 1);
            Setting.instance.numFlatStake = readDoubleKey("numFlatStake", 1);
            Setting.instance.isFlatStake = readBoolKey("isFlatStake", true);
            Setting.instance.isRecordResult = readBoolKey("isEnabledH2H", false);
            Setting.instance.isEnabled3PAUS = readBoolKey("isEnabled3PAUS", false);
            Setting.instance.numBetCount = readDoubleKey("numBetCount", 100);
            Setting.instance.numOddsMin = readDoubleKey("numOddsMin", 1.3);
            Setting.instance.heightDiff = readDoubleKey("heightDiff", 71);

            Setting.instance.isEnabledChrome  = readBoolKey("isEnabledChrome", true);
            Setting.instance.isEnabledFirefox = readBoolKey("isEnabledFirefox", false);
            Setting.instance.isEnabledEdge    = readBoolKey("isEnabledEdge", false);

            Setting.instance.chromePath       = ReadCommonRegistry("chromePath");
            Setting.instance.firefoxPath      = ReadCommonRegistry("firefoxPath");
            Setting.instance.edgePath         = ReadCommonRegistry("edgePath");

            Setting.instance.isUseUILogin = readBoolKey("isUseUILogin", false);
            Setting.instance.delayAfterLogin = readDoubleKey("delayAfterLogin", 5);
            Setting.instance.delayAfterRefresh = readDoubleKey("delayAfterRefresh", 3);
            Setting.instance.delayBetweenRetries = readDoubleKey("delayBetweenRetries", 3);
            Setting.instance.delayLoad_Login = readDoubleKey("delayLoad_Login", 3);
            Setting.instance.delayBetweenBets = readDoubleKey("delayBetweenBets", 3);
            Setting.instance.delayStart_Load = readDoubleKey("delayStart_Load", 3);

            Setting.instance.placingMode = (PLACING_MODE)((int)readDoubleKey("placingMode", 0));

            //Dolphin
            Setting.instance.dolphinUrl = readStringKey("dolphinUrl");
            Setting.instance.dolphinToken = readStringKey("dolphinToken");
            Setting.instance.dolphinProfileId = readStringKey("dolphinProfileId");

        }

        private bool readBoolKey(string keyname, bool def = false)
        {
            try
            {
                string value = ReadRegistry(keyname);
                if (string.IsNullOrEmpty(value)) return def;
                return value == "true" ? true : false; ;
            }
            catch
            {
            }
            return def;
        }

        private double readDoubleKey(string keyname, double def = 0)
        {
            try
            {
                string value = ReadRegistry(keyname);
                if (string.IsNullOrEmpty(value)) return def;
                return Utils.ParseToDouble(value);
            }
            catch
            {
            }
            return def;
        }

        private string readStringKey(string keyname, string def = "")
        {
            try
            {
                string value = ReadRegistry(keyname);
                if (string.IsNullOrEmpty(value)) return def;
                return value;
            }
            catch
            {

            }
            return def;
        }


    }
}
