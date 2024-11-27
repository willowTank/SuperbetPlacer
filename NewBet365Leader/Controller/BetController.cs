using FirefoxBet365Placer.Constants;
using FirefoxBet365Placer.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirefoxBet365Placer.Controller
{
    public class BetController
    {
        protected static BetUIController _instance = null;

        protected onWriteStatusEvent m_handlerWriteStatus;
        protected onWriteLogEvent m_handlerWriteLog;
        protected SocketConnector m_socketConnector;
        public bool _isInjected = false;

        public bool WaitingForLogin = false;
        public bool PlacingBet = false;
        public string RespBody = string.Empty;
        private bool _isAddbetRequested;
        // for bet
        protected double _lastMaxStake = 0;
        protected const int _TIMEOUT_SESSION_LOCKED = 5000;
        protected const int _TIMEOUT_ODDS_CHANGED = 6000;
        protected const int _TIMEOUT_ZERO_MS = 3000;

        protected List<BetItem> _retryBetList = new List<BetItem>();
        public bool _isPageLoaded { get; set; }

        private const string strLoginScript = "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";
        private const string strInjectScript = "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";
        protected string _GUID = string.Empty;
        private object _lockHandler = new object();

        string[] visitUrlList = new string[]
        {
            "https://www." + Setting.instance.bet365Domain + "/#/HO/",
            "https://www." + Setting.instance.bet365Domain + "/#/IP/",
            "https://www." + Setting.instance.bet365Domain + "/#/IP/B2",
            "https://www." + Setting.instance.bet365Domain + "/#/IP/B4",
            "https://www." + Setting.instance.bet365Domain + "/#/IP/B1",
            "https://www." + Setting.instance.bet365Domain + "/#/AS/B1/",
            "https://www." + Setting.instance.bet365Domain + "/#/AS/B2/",
            "https://www." + Setting.instance.bet365Domain + "/#/AS/B4/",
        };
        protected int visitIndex = 0;
        private bool isArrivedJSResult;
        private string _jsResult;
        protected bool _isLastLoginModalClicked = false;

        public static BetUIController Intance
        {
            get
            {
                return _instance;
            }
        }

        public bool LOGGED { get; internal set; }
        public DateTime _lastCallPlaceBet { get; set; }
        public bool WaitingForAPI { get; set; }
        public Panel Parent { get; set; }
        private string syncTerm { get; set; }
        public bool ClickingPlaceBet { get; private set; }

        public Dictionary<string, string> _jsResultDict = new Dictionary<string, string>();

        public BetController(onWriteStatusEvent _handlerWriteStatus, onWriteLogEvent _handlerWriteLog, SocketConnector _socketConnector)
        {
            m_handlerWriteStatus = _handlerWriteStatus;
            m_handlerWriteLog = _handlerWriteLog;
            m_socketConnector = _socketConnector;
            _GUID = Guid.NewGuid().ToString();
            ClickingPlaceBet = false;
        }

        public void WebResourceResponseReceived(JObject message)
        {
            try
            {
                string workingURL = message.SelectToken("url").ToString().ToLower();
                string responseBody = message.SelectToken("response").ToString();
                if(workingURL == "jsresult")
                {
                    isArrivedJSResult = true;
                    _jsResult = responseBody;

                    string hash = "0";
                    if (message.SelectToken("hash") != null)
                    {
                        hash = message.SelectToken("hash").ToString();
                    }
                    lock (_lockHandler)
                    {
                        if (_jsResultDict.ContainsKey(hash)) _jsResultDict.Remove(hash);
                        _jsResultDict.Add(hash, responseBody);
                    }
                }
                if (workingURL.Contains("recaptcha/enterprise.js?render="))
                {
                    _isPageLoaded = true;
                }
                else if (workingURL.Contains("uicountersapi"))
                {
                    _isPageLoaded = true;
                }
                else if (workingURL.Contains("recaptcha/enterprise"))
                {
                    _isPageLoaded = true;
                }
                else if (workingURL.Contains("defaultapi/sports-configuration"))
                {
                    WaitingForLogin = false;
                    if (responseBody.ToLower().Contains(Setting.instance.betUsername.ToLower()))
                        LOGGED = true;
                    else
                        LOGGED = false;
                }
                else if (workingURL.Contains("betswebapi"))
                {
                    m_handlerWriteStatus(workingURL);
                    m_handlerWriteStatus(responseBody);
                    RespBody = responseBody;
                    WaitingForAPI = false;
                    if (workingURL.Contains("placebet"))
                    {
                        RespBody = responseBody;
                        WaitingForAPI = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        public void DoVisitOtherPage(int index = -1)
        {
            try
            {
                if (index == -1)
                {
                    if (visitUrlList.Length <= visitIndex) visitIndex = 0;
                    NavigateInvoke(visitUrlList[visitIndex++]);
                }
                else
                {
                    visitIndex = index;
                    NavigateInvoke(visitUrlList[index]);
                }

            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
            }
        }

        public JArray getSettledBets()
        {
            JArray jsonData = new JArray();
            try
            {
                string urlMyBet = "https://www." + Setting.instance.bet365Domain + "/#/MB/";
                ExecuteScript(string.Format("location.href='{0}'", urlMyBet));
                Thread.Sleep(5000);
                
                string jsCode = "var menuList = document.getElementsByClassName('myb-HeaderButton ');menuList[menuList.length-2].click();";
                ExecuteScript(jsCode);
                Thread.Sleep(2000);

                jsCode = "var OpenBetList = [];var e, n;if (n = Locator.treeLookup.getReference('SETTLEDBETS')){for (e = 0; e < n.getChildren().length; e++){var OpenBetItr = n.getChildren()[e].data;data = [];var m;for (m = 0; m < n.getChildren()[e].getChildren().length; m++){data.push(n.getChildren()[e].getChildren()[m].data);}OpenBetItr['data'] = data;OpenBetList.push(OpenBetItr);}}; JSON.stringify(OpenBetList);";
                string jsResult = ExecuteScript(jsCode, true);
                jsResult = jsResult.Replace("\\\"", "\"");
                jsResult = jsResult.Substring(1, jsResult.Length - 2).Trim();
                jsResult = "[" + jsResult + "]";
                m_handlerWriteStatus(jsResult);
                jsonData = JsonConvert.DeserializeObject<JArray>(jsResult);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
            }
            return jsonData;
        }

        public static void CreateInstance(onWriteStatusEvent _handlerWriteStatus, onWriteLogEvent _handlerWriteLog, SocketConnector _socketConnector)
        {
            _instance = new BetUIController(_handlerWriteStatus, _handlerWriteLog, _socketConnector);
        }

        private void postTokenToServers(string payload)
        {
            CustomEndpoint.sendNewUserTerm("http://89.40.6.53:6002", payload);
            CustomEndpoint.sendNewUserTerm("http://91.121.70.201:9002", payload);
        }

        public void DoLoad365Page(string visitUrl = "")
        {
            try
            {
                CancellationTokenSource canceller = new CancellationTokenSource();
                var cancellerToken = canceller.Token;
                Task t = Task.Run(() =>
                {
                    _isPageLoaded = false;
                    LOGGED = false;
                    _isInjected = false;
                    if (string.IsNullOrEmpty(visitUrl))
                        visitUrl = Setting.instance.bet365Domain + "/#/IP/B2";
                    int retryCount = 10;
                    while (--retryCount > 0 && !cancellerToken.IsCancellationRequested)
                    {
                        try
                        {
                            NavigateInvoke(visitUrl, true);
                            while (!_isPageLoaded && !CheckIfJSLibLoaded()) Thread.Sleep(100);
                            m_handlerWriteStatus("DoLoad365Page => 365 page is loaded!");
                            break;
                        }
                        catch
                        {
                        }
                    }

                }, cancellerToken);

                TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 15);
                if (!t.Wait(ts))
                {
                    canceller.Cancel();
                    m_handlerWriteStatus("The timeout interval elapsed in DoLoad365Page of BetController.");
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in DoLoad365Page: " + ex.ToString());
            }
        }

        public bool CheckIfJSLibLoaded()
        {
            try
            {
                string jsCode = "JSON.stringify(ns_betslipcorelib_util)";
                string jsResult = ExecuteScript(jsCode, true);
                //m_handlerWriteStatus(jsResult);
                if (jsResult.Contains("{}"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in CheckIfJSLibLoaded: " + ex.ToString());
                if (ex.ToString().Contains("ns_betslipcorelib_util is not defined"))
                {
                    return false;
                }
            }
            return false;
        }


        public void DoLoginOld(string visitUrl = "")
        {
            try
            {
                m_handlerWriteStatus("DoLogin");
                DoLoad365Page(visitUrl);
                if (CheckIfLogged()) return;
                string loginScript = string.Format("doLogin365('{0}', '{1}', '{2}')",
                    Setting.instance.betUsername, Setting.instance.betPassword, Setting.instance.bet365Domain);

                Task t = Task.Run(async () =>
                {
                    LOGGED = false;
                    int retryCount = 5;
                    while (--retryCount > 0)
                    {
                        WaitingForLogin = true;
                        ExecuteScript(loginScript);
                        m_handlerWriteStatus(loginScript);
                        m_handlerWriteStatus("Executed login script!");
                        while (WaitingForLogin && !CheckIfLogged()) Thread.Sleep(200);
                        Thread.Sleep(1000);
                        if (CheckIfLogged()) break;
                        Thread.Sleep(1000);
                    }

                    m_handlerWriteStatus("Login Success!");
                    _isLastLoginModalClicked = false;
                    Thread.Sleep(3000);
                    m_handlerWriteStatus("Page is loaded and waiting for 30 seconds!");
                    Thread.Sleep(6000);
                    DoClickDlgBox();
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 60);
                if (!t.Wait(ts))
                {
                    m_handlerWriteStatus("The timeout interval elapsed in DoLogin of BetController.");
                }

            }
            catch
            {

            }
            WaitingForLogin = false;
        }

        public bool DoLogin(string visitUrl = "")
        {
            bool bRet = false;
            try
            {
                DoLoad365Page(visitUrl);
                if (CheckIfLogged()) return true;
                m_handlerWriteStatus("DoLogin");
                string loginScript = string.Format("doLogin365('{0}', '{1}', '{2}')", Setting.instance.betUsername, Setting.instance.betPassword, Setting.instance.bet365Domain);
                CancellationTokenSource canceller = new CancellationTokenSource();
                var cancellerToken = canceller.Token;
                Task t = Task.Run(async () =>
                {
                    LOGGED = false;
                    WaitingForLogin = true;
                    ExecuteScript(loginScript);
                    m_handlerWriteStatus(loginScript);
                    m_handlerWriteStatus("Executed login script!");
                    Thread.Sleep(3000);

                    while (WaitingForLogin && !CheckIfLogged() && !cancellerToken.IsCancellationRequested) Thread.Sleep(200);
                    if (CheckIfLogged())
                    {
                        bRet = true;
                        
                    }
                    else
                    {
                        m_handlerWriteStatus("Login Failure!");
                        bRet = false;
                    }
                }, cancellerToken);

                TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 15);
                if (!t.Wait(ts))
                {
                    canceller.Cancel();
                    m_handlerWriteStatus("The timeout interval elapsed in DoLogin of BetController.");
                }
                if (bRet)
                {
                    m_handlerWriteStatus("Login Success!");
                    m_handlerWriteStatus("Page is loaded and waiting for36 seconds!");
                    Thread.Sleep(3000);
                    _isLastLoginModalClicked = false;
                    DoClickDlgBox();
                }
            }
            catch
            {
            }
            WaitingForLogin = false;
            return bRet;
        }

        public bool DoClickDlgBox()
        {
            try
            {
                if (_isLastLoginModalClicked == false)
                {
                    string result = ExecuteScript("getPosLastLoginModule()", true);
                    if (result != "false" && result != "")
                    {
                        //m_handlerWriteStatus("getPosLastLoginModule() = " + result);
                        JObject btnPosition = JsonConvert.DeserializeObject<JObject>(result);
                        decimal x = decimal.Parse(btnPosition.SelectToken("x").ToString());
                        decimal y = decimal.Parse(btnPosition.SelectToken("y").ToString());
                        if (x == 0 && y == 0)
                        {
                            m_handlerWriteStatus("Wrong position detected!");
                        }
                        else
                        {
                            FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                            FakeUserAction.Intance.SimClick();
                            result = ExecuteScript("getPosLastLoginModule()", true);
                            if (result == "false" || result == "")
                            {
                                m_handlerWriteStatus("getPosLastLoginModule() = closed");
                                _isLastLoginModalClicked = true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }


            try
            {
                string result = ExecuteScript("getPosUnreadMessage()", true);
                if (result != "false" && result != "")
                {
                    //m_handlerWriteStatus("getPosLastLoginModule() = " + result);
                    JObject btnPosition = JsonConvert.DeserializeObject<JObject>(result);
                    decimal x = decimal.Parse(btnPosition.SelectToken("x").ToString());
                    decimal y = decimal.Parse(btnPosition.SelectToken("y").ToString());
                    if (x == 0 && y == 0)
                    {
                        m_handlerWriteStatus("Wrong position detected!");
                    }
                    else
                    {
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                        FakeUserAction.Intance.SimClick();
                        result = ExecuteScript("getPosUnreadMessage()", true);
                        if (result == "false" || result == "")
                        {
                            m_handlerWriteStatus("getPosUnreadMessage() = closed");
                            _isLastLoginModalClicked = true;
                        }
                    }
                }
            }
            catch
            {

            }

            return true;
        }

        async public Task<bool> DoClickDlgBoxByScript()
        {
            try
            {

                string strScript = "document.getElementsByClassName('llm-LastLoginModule_Button ')[0].click()";
                ExecuteScript(strScript);
                Thread.Sleep(100);
            }
            catch
            {
            }
            try
            {
                string strScript = "document.getElementsByClassName('pm-MessageOverlayCloseButton ')[0].click()";
                ExecuteScript(strScript);
            }
            catch
            {
            }

            try
            {
                string strScript = "document.getElementsByClassName('bss-ReceiptContent_Done ')[0].click()";
                ExecuteScript(strScript);
            }
            catch
            {
            }

            try
            {
                string strScript = "document.getElementsByClassName('bss-NormalBetItem_Remove')[0].click()";
                ExecuteScript(strScript);
            }
            catch
            {
            }

            try
            {
                string strScript = "document.getElementsByClassName('bss-RemoveButton ')[0].click()";
                ExecuteScript(strScript);
            }
            catch
            {
            }

            try
            {
                string strScript = "document.getElementsByClassName('bss-MultipleHeader_Remove')[0].click()";
                ExecuteScript(strScript);
            }
            catch
            {
            }


            return true;
        }

        public int GetOpenBetCount()
        {
            int betCount = 0;
            try
            {
                string jsCode = "Locator.treeLookup.getReference('OPENBETS').data.BO";
                string jsResult = ExecuteScript(jsCode, true);
                jsResult = jsResult.Replace("'", "");
                betCount = int.Parse(jsResult);
            }
            catch
            {

            }
            return betCount;
        }

        public double GetBalance() 
        {
            try
            {
                string strBalance = ExecuteScript("Locator.user.getBalance().totalBalance", true);
                //m_handlerWriteStatus(strBalance);
                if (string.IsNullOrEmpty(strBalance) || strBalance == "null" || strBalance == "undefined") return -1;
                strBalance = strBalance.Replace("\"", "").Replace("'", "");
                strBalance = strBalance.Substring(0, strBalance.Length - 1);
                double balance = Utils.ParseToDouble(strBalance.Replace(",", "").Replace(",", "."));
                //m_handlerWriteStatus(string.Format("Current balance : {0}", balance));
                return balance;
            }
            catch
            {

            }
            return -1;
            
        }

        public string ExecuteScript(string jsCode, bool requiredResult = false)
        {
            string result = string.Empty;
            try
            {
                if (ExecuteScriptBase("IsJsInjected===true?'yes':'no'", true) == "no")
                {
                    try
                    {
                        string INJECTED_JS_CODE = File.ReadAllText("inject.txt");
                        INJECTED_JS_CODE = Utils.Decrypt(INJECTED_JS_CODE);
                        ExecuteScriptBase(INJECTED_JS_CODE);
                        lock (_lockHandler)
                        {
                            _jsResultDict.Clear();
                        }
                    }
                    catch
                    {
                    }
                    
                }
                result = ExecuteScriptBase(jsCode, requiredResult);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
            }
            return result;
        }

        public string ExecuteScriptBaseOld(string jsCode, bool requiredResult = false)
        {
            string result = string.Empty;
            try
            {
                //WebsocketClient.Instance.ExecuteScript(jsCode);
                isArrivedJSResult = false;
                _jsResult = string.Empty;
                WebsocketServer.Instance.ExecuteScript(jsCode);
                if (!requiredResult) return result;
                Task t = Task.Run(() =>
                {
                    
                    while (!isArrivedJSResult) Thread.Sleep(10);
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 5);
                if (!t.Wait(ts))
                {
                    //m_handlerWriteStatus("The timeout interval elapsed at BetController::ExecuteScript");
                }
                result = _jsResult;
            }
            catch (Exception ex)
            {
                //m_handlerWriteStatus(ex.ToString());
            }
            return result;
        }

        public string ExecuteScriptBase(string jsCode, bool requiredResult = false)
        {
            string result = string.Empty;
            string hash = jsCode.GetHashCode().ToString();
            try
            {
                //WebsocketClient.Instance.ExecuteScript(jsCode);
                lock (_lockHandler)
                {
                    try { _jsResultDict.Remove(hash); } catch { }
                }
                WebsocketServer.Instance.ExecuteScript(jsCode);
                if (!requiredResult) return result;
                Task t = Task.Run(() =>
                {
                    isArrivedJSResult = false;
                    _jsResult = string.Empty;
                    while (true)
                    {
                        lock (_lockHandler)
                        {
                            if (_jsResultDict.ContainsKey(hash)) break;
                        }
                        Thread.Sleep(100);
                    }

                });
                TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 5);
                if (!t.Wait(ts))
                {
                    //m_handlerWriteStatus("The timeout interval elapsed at BetController::ExecuteScript");
                }
                lock (_lockHandler)
                {
                    if (_jsResultDict.ContainsKey(hash))
                        result = _jsResultDict[hash];
                    else
                        result = _jsResult;
                }
                isArrivedJSResult = false;
                _jsResult = string.Empty;
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
            }
            return result;
        }

        public bool NavigateInvoke(string visitUrl, bool bFresh  = false)
        {
            try
            {
                if (!visitUrl.StartsWith("https://")) visitUrl = "https://www." + visitUrl;
                m_handlerWriteStatus(string.Format("NavigateInvoke => {0}", visitUrl));
                if (bFresh)
                {
                    ExecuteScript(string.Format("location.href==='{0}'", visitUrl));
                }
                else
                {
                    ExecuteScript(string.Format("location.href==='{0}'?0:location.href='{0}'", visitUrl));
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
                int a = 1;
            }
            return true;
        }
        public bool CheckIfLogged()
        {
            try
            {
                string jsResult = ExecuteScript("flashvars.LOGGED_IN", true);
                if (string.IsNullOrEmpty(jsResult)) return false;
                if (jsResult.ToLower().Contains("true"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteLog("Exception in CheckIfLogged: " + ex.ToString());
            }
            return false;
        }

        protected string calculateSA()
        {
            Random rnd = new Random();
            int randVal = rnd.Next(1, 15);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + randVal;
            string aa = unixTimestamp.ToString("X2").ToLower();
            string hexValue = DateTime.Now.Ticks.ToString("X2");
            return aa + "-" + hexValue.Substring(hexValue.Length - 8, 8);
        }

        protected string convertBStoMobile(BetItem betitem, double amount = 0, bool bEW = false, string odds = "", string sa = "")
        {
            string ret = string.Empty;
            string bs = betitem.bs;
            try
            {
                string o = Setting.instance.Between(bs, "#o=", "#");
                string c = Setting.instance.Between(bs, "#c=", "#");
                string ew = Setting.instance.Between(bs, "#ew=", "#");
                if (odds != "")
                {
                    o = odds;
                }
                if (!o.Contains("/")) o = "1/3";
                string f = Setting.instance.Between(bs, "#f=", "#");
                string fp = Setting.instance.Between(bs, "#fp=", "#");
                if (sa == "")
                {
                    sa = calculateSA();
                }
                string st = Setting.instance.Between(bs, "#st=", "#");
                string ln = Setting.instance.Between(bs, "#ln=", "#");
                string ust = Setting.instance.Between(bs, "#ust=", "#");
                if (amount > 0)
                {
                    //if (ew == "1") amount = amount / 2;
                    st = amount.ToString("F2");
                    ust = amount.ToString("F2");
                }
                double profit = Math.Floor((100 * amount * Utils.FractionToDouble(o))) / 100;
                string tr = profit.ToString("F2");
                if (bs.Contains("ln"))
                {
                    ret = string.Format("pt=N#o={0}#f={1}#fp={2}#so=#c=1#mt=16#ln={6}#sa={3}#|TP=BS{1}-{2}#st={4}#ust={4}#tr={7}#||",
                    o, f, fp, sa, st, ust, ln, tr);
                }
                else if (bEW || betitem.bEW)
                {
                    ret = string.Format("pt=N#o={0}#f={1}#fp={2}#so=#c=1#mt=16#sa={3}#|TP=BS{1}-{2}#st={4}#ust={4}#tr={6}#ew=1#||",
                    o, f, fp, sa, st, ust, tr);
                }
                else if (betitem.source == Constants.SOURCE.BASHING)
                {
                    tr = "0.00";
                    if (bEW || betitem.bEW)
                    {
                        ret = string.Format("pt=N#o={0}#f={1}#fp={2}#so=#c={7}#mt=13#sa={3}#|TP=BS{1}-{2}#st={4}#ust={4}#fb={6}#tr={6}#ew=1#||", o, f, fp, sa, st, ust, tr, c);
                    }
                    else
                    {
                        ret = string.Format("pt=N#o={0}#f={1}#fp={2}#so=#c={7}#mt=13#sa={3}#|TP=BS{1}-{2}#st={4}#ust={4}#fb={6}#tr={6}#||", o, f, fp, sa, st, ust, tr, c);
                    }

                }
                else
                {
                    ret = string.Format("pt=N#o={0}#f={1}#fp={2}#so=#c={7}#mt=16#sa={3}#|TP=BS{1}-{2}#st={4}#ust={4}#tr={6}#||",
                    o, f, fp, sa, st, ust, tr, c);
                }

            }
            catch (Exception ex)
            {

            }
            return ret;
        }
        public void clearRetryBets()
        {
            try
            {
                _retryBetList.Clear();
            }
            catch
            {
            }
        }

        public void ClearCookie()
        {
            try
            {
                m_handlerWriteStatus("Cookies are cleared!");
                //m_browser.Context.ClearCookiesAsync();
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in ClearCookie: " + ex.ToString());
            }
        }

        private void replaceSA(dynamic jsonSlipResponse, ref string ns)
        {
            try
            {
                m_handlerWriteStatus("Before replaceSA: ns => " + ns);
                string[] ptList = ns.Split(new string[] { "pt=N" }, StringSplitOptions.None);
                for (int i = 1; i < ptList.Length; i++)
                {
                    string saFromAddBet = jsonSlipResponse.bt[i - 1].sa.ToString();
                    ptList[i] = Utils.ReplaceStr(ptList[i], saFromAddBet, "#sa=", "#");
                }
                ns = string.Join("pt=N", ptList);
            }
            catch
            {

            }
            m_handlerWriteStatus("After replaceSA: ns => " + ns);
        }

        protected bool IsInOddsThreshold(BetItem betitem, double oldOdds, double currentOdds)
        {
            if (betitem.source == Constants.SOURCE.DOG_PREMATCH)
            {
                if (currentOdds > 20) return true;
            }
            else if (betitem.source == Constants.SOURCE.RACING_INVEST)
            {
                if (currentOdds >= betitem.minOdds)
                {
                    return true;
                }
            }
            else if (betitem.source == Constants.SOURCE.TRADEMATE)
            {
                if (currentOdds >= oldOdds || 100 * Math.Abs(currentOdds - oldOdds) / oldOdds < betitem.oddsDistance)
                {
                    return true;
                }
            }
            else if (betitem.source == Constants.SOURCE.BETBURGER)
            {
                if (currentOdds >= oldOdds || Math.Abs(currentOdds - oldOdds) / oldOdds < 0.1)
                {
                    return true;
                }
            }
            else if (betitem.source == SOURCE.TIPSTER)
            {
                if (currentOdds >= oldOdds || Math.Abs(currentOdds - oldOdds) / oldOdds < 0.5)
                {
                    return true;
                }
            }
            else if (betitem.source == Constants.SOURCE.DOGWIN || betitem.source == Constants.SOURCE.USAHORSE || betitem.source == Constants.SOURCE.BASHING)
            {
                if (currentOdds >= oldOdds || Math.Abs(currentOdds - oldOdds) / oldOdds < 0.03)
                {
                    return true;
                }
            }
            else
            {
                if (currentOdds >= oldOdds || Math.Abs(currentOdds - oldOdds) < 0.04)
                    return true;
                double PvsThis = (1 + betitem.oddsDistance / 100) / oldOdds;
                double newValue = 100 * (PvsThis * currentOdds - 1);
                betitem.arbPercent = newValue;
                m_handlerWriteStatus(string.Format("Old odds: {0} New Odds : {1} Old value: {2} New value: {3}", oldOdds, currentOdds, betitem.oddsDistance, newValue));
                double minValue = 8;
                if (newValue >= minValue) return true;
            }
            m_handlerWriteStatus(string.Format("Old odds: {0} New Odds : {1}", oldOdds, currentOdds));
            return false;
        }

        public bool handleOddsChange(ref BetItem betitem, ref string strBody, dynamic jsonContent, ref double newOdds, ref double oldOdds, ref double newHandicap, ref double oldHandicap)
        {
            m_handlerWriteStatus(string.Format("handleOddsChange => strBody: {0}, betitem.bs : {1}", strBody, betitem.bs));

            string[] oldPTList = strBody.Split(new string[] { "pt=N" }, StringSplitOptions.None);
            string[] startPTList = betitem.bs.Split(new string[] { "pt=N" }, StringSplitOptions.None);
            int i = 1;
            foreach (var item in jsonContent.bt)
            {
                //Odds changed
                if (item.od != null)
                {
                    string strNewOdds = item.od.ToString();
                    newOdds = Utils.FractionToDouble(strNewOdds);
                    oldOdds = Utils.FractionToDouble(Setting.instance.Between(startPTList[i], "#o=", "#"));
                    m_handlerWriteStatus(string.Format("New Odds: {0} Old Odds: {1}", newOdds, oldOdds));
                    if (!IsInOddsThreshold(betitem, oldOdds, newOdds)) return false;
                    oldPTList[i] = Setting.instance.ReplaceStr(oldPTList[i], strNewOdds, "#o=", "#");
                    //double stake = Utils.ParseToDouble(Setting.instance.Between(oldPTList[i], "#st=", "#"));
                    //oldPTList[i] = Setting.instance.ReplaceStr(oldPTList[i], (stake * newOdds).ToString("N2"), "#tr=", "#");
                }

                /*
                // Return changed
                if (item.re != null)
                {
                    string newReturn = item.re.ToString();
                    oldPTList[i] = Setting.instance.ReplaceStr(oldPTList[i], newReturn, "#tr=", "#");
                }
                */

                //handicap changed
                if (item.pt[0].hd != null)
                {
                    string newHD = item.pt[0].ha.ToString();
                    newHandicap = Utils.ParseToDouble(newHD);
                    oldHandicap = Utils.ParseToDouble(Setting.instance.Between(oldPTList[i], "#ln=", "#"));

                    oldHandicap = Utils.ParseToDouble(Setting.instance.Between(startPTList[i], "#ln=", "#"));
                    //oldHandicap = Utils.ParseToDouble(betitem.handicap);
                    string[] pickList = betitem.pick.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                    m_handlerWriteStatus(string.Format("New Handicap: {0} Old Handicap: {1}", newHandicap, oldHandicap));

                    if (pickList[i - 1].ToLower().Contains("over") || betitem.pick.Contains("Más de"))
                    {
                        newHandicap = 0 - newHandicap;
                        oldHandicap = 0 - oldHandicap;
                    }
                    if (newHandicap < oldHandicap)
                    {
                        return false;
                    }
                    oldPTList[i] = Setting.instance.ReplaceStr(oldPTList[i], newHD, "#ln=", "#");
                }
                i++;
            }
            strBody = string.Join("pt=N", oldPTList);

            if (jsonContent.mo == null) return true;
            try
            {
                foreach (var item in jsonContent.mo)
                {
                    string ms = Setting.instance.Between(strBody, "id=" + item.bt.ToString(), "||");
                    if (item.st != null)
                    {
                        string newST = item.st.ToString();
                        ms = Setting.instance.ReplaceStr(ms, newST, "#st=", "#");
                        ms = Setting.instance.ReplaceStr(ms, newST, "#ust=", "#");
                    }
                    if (item.re != null)
                    {
                        string newRE = item.re.ToString();
                        ms = Setting.instance.ReplaceStr(ms, newRE, "#tr=", "#");
                    }
                    strBody = Setting.instance.ReplaceStr(strBody, ms, "id=" + item.bt.ToString(), "||");
                }
            }
            catch
            {
            }
            return true;
        }


        public bool handleOddsChangeUI(ref BetItem betitem, dynamic jsonContent, ref double newOdds, ref double oldOdds, ref double newHandicap, ref double oldHandicap)
        {
            try
            {
                m_handlerWriteStatus("handleOddsChangeUI");
                int i = 1;
                foreach (dynamic item in jsonContent.bt)
                {
                    if (item.ToString().Contains(betitem.runnerId) == false) continue;
                    //Odds changed
                    if (item.od != null)
                    {
                        string strNewOdds = item.od.ToString();
                        newOdds = Utils.FractionToDouble(strNewOdds);
                        oldOdds = betitem.odds;
                        m_handlerWriteStatus(string.Format("New Odds: {0} Old Odds: {1}", newOdds, oldOdds));
                        if (!IsInOddsThreshold(betitem, oldOdds, newOdds)) return false;
                    }


                    //handicap changed
                    if (item.pt[0].hd != null)
                    {
                        string newHD = item.pt[0].ha.ToString();
                        newHandicap = Utils.ParseToDouble(newHD);
                        oldHandicap = betitem.line;
                        //oldHandicap = Utils.ParseToDouble(betitem.handicap);
                        //string[] pickList = betitem.pick.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                        m_handlerWriteStatus(string.Format("New Handicap: {0} Old Handicap: {1}", newHandicap, oldHandicap));

                        //if (pickList[i - 1].ToLower().Contains("over") || betitem.pick.Contains("Más de"))
                        //{
                        //    newHandicap = 0 - newHandicap;
                        //    oldHandicap = 0 - oldHandicap;
                        //}
                        if (newHandicap != oldHandicap)
                        {
                            return false;
                        }
                    }
                    i++;
                }
                return true;
            }
            catch
            {

            }
            return false;
        }

        private string changeStakePart(string ns, double newStake)
        {
            try
            {
                m_handlerWriteStatus("old ns=>" + ns);
                string[] oldPTList = ns.Split(new string[] { "pt=N" }, StringSplitOptions.None);
                for (int i = 0; i < oldPTList.Length; i++)
                {
                    string bs = oldPTList[i];
                    if (!bs.Contains("#st=")) continue;
                    double stake = Utils.ParseToDouble(Setting.instance.Between(bs, "#st=", "#").Replace(",", "."));
                    if (newStake > 1)
                    {
                        if (Setting.instance.isComplex)
                            stake = newStake;
                        else
                            stake = Math.Floor(newStake);
                    }
                    else
                        stake = newStake;

                    bs = Setting.instance.ReplaceStr(bs, stake.ToString("N2"), "#st=", "#");
                    bs = Setting.instance.ReplaceStr(bs, stake.ToString("N2"), "#ust=", "#");
                    oldPTList[i] = bs;

                    if (!bs.Contains("#o=")) continue;
                    double odds = Utils.FractionToDouble(Setting.instance.Between(bs, "#o=", "#"));
                    double tr = (odds * stake);
                    bs = Setting.instance.ReplaceStr(bs, tr.ToString("N2"), "#tr=", "#");
                    bs = Setting.instance.ReplaceStr(bs, "", "#tr=", "#");
                    oldPTList[i] = bs;
                }
                ns = string.Join("pt=N", oldPTList);
                m_handlerWriteStatus("new ns=>" + ns);
                return ns;
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Error during changeStakePart: " + ex.ToString());
            }
            return ns;
        }

    }
}
