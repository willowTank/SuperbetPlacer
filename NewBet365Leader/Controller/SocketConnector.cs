using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Socket.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;
using FirefoxBet365Placer.Json;
using FirefoxBet365Placer.Constants;
using System.Windows.Forms;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp;
using PuppeteerSharp;
using Windows.Media.Protection.PlayReady;
using System.Security.Policy;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;

namespace FirefoxBet365Placer.Controller
{

    public enum VALIDATE_CODE
    {
        SUCCESS,
        NOEXIST_KEY,
        INUSE_KEY,
        PAUSED_KEY,
        NEW_KEY,
        INVALID
    }

    public class SocketConnector
    {
        private SocketIoClient _socket = null;
        private Thread m_pingThread = null;
        private onWriteLogEvent m_handlerWriteLog;
        private onWriteStatusEvent m_handlerWriteStatus;
        private onProcNewTipEvent m_handlerProcNewtip;
        public onProcUpdateNetworkStatusEvent m_handlerProcUpdateNetworkStatus;

        public List<BetItem> m_placedBetList = new List<BetItem>();
        public List<List<BetItem>> m_placedComboBetList = new List<List<BetItem>>();

        public int countOfTodayBashBet { get; set; }

        public List<BetItem> m_oddsChangedBetList = new List<BetItem>();

        public List<BetItem> m_receivedBetList = new List<BetItem>();

        private string _KEY = string.Empty;
        private string _IV = string.Empty;
        private Thread _reconnectThread;

        private HttpClient client;
        static public IPage page;
        public float balance;
        public SocketConnector(onWriteLogEvent onWriteLog, onWriteStatusEvent onWriteStatus, onProcNewTipEvent onProcNewTipEvent)
        {
            m_handlerWriteLog = onWriteLog;
            m_handlerWriteStatus = onWriteStatus;
            m_handlerProcNewtip = onProcNewTipEvent;
            Init();
        }

        public void CloseSocket()
        {
            try
            {
                _socket.DisconnectAsync();
            }
            catch
            {
            }
        }

        private void pingThreadFunc()
        {
            while (true)
            {
                try
                {
                    if (GlobalConstants.validationState != ValidationState.SUCCESS)
                    {
                        SendPresentInfo(-1);
                    }

                    if (m_placedBetList.Count > 1000)
                    {
                        m_placedBetList = m_placedBetList.GetRange(100, 999);
                        IOManager.saveBetData(m_placedBetList);
                    }
                    if (m_oddsChangedBetList.Count > 1000)
                        m_oddsChangedBetList = m_oddsChangedBetList.GetRange(100, 999);
                }
                catch
                {
                }
                Thread.Sleep(2000);
            }
        }

        public async void Init()
        {
            m_placedBetList = IOManager.readBetData();
            m_handlerWriteStatus(string.Format("m_placedBetList.length={0}", m_placedBetList.Count));
            countOfTodayBashBet = getCountOfTodayBets();
            client = new HttpClient();
            dolphinAPi();
        }

        public void addPlacedbet(List<BetItem> betitemList)
        {
            Guid guid = Guid.NewGuid();

            foreach (BetItem itemX in betitemList)
            {
                itemX.betId = guid.ToString();
                itemX.placedDate = DateTime.Now;
                m_placedBetList.Add(itemX);
            }
        }

        public int getCountOfTodayBets()
        {
            int todayCount = 0;
            DateTime today = DateTime.Now;
            foreach (BetItem betitem in m_placedBetList)
            {
                if (betitem.source != SOURCE.BASHING) continue;
                DateTime placedDate = betitem.placedDate;
                if (placedDate.Year == today.Year &&
                    placedDate.Month == today.Month &&
                    placedDate.Day == today.Day)
                {
                    todayCount++;
                }
            }
            return todayCount;
        }

        public bool checkPlacedDoublebet(List<BetItem> betitemList)
        {
            BetItem foundItem = null;
            foreach (BetItem itemX in betitemList)
            {
                for (int i = 0; i < m_placedBetList.Count; i++)
                {
                    if (m_placedBetList[i].runnerId == itemX.runnerId)
                    {
                        foundItem = m_placedBetList[i];
                    }
                }
            }
            if (foundItem != null)
            {
                foreach (BetItem itemX in betitemList)
                {
                    if (!foundItem.runnerIdList.Contains(itemX.runnerId))
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool isBlackList(BetItem newitemA, BetItem newitemB)
        {
            BetItem foundItemA = null;
            foreach (var item in m_placedBetList)
            {
                if (newitemA.match.Equals(item.match))
                {
                    BetItem foundItemB = m_placedBetList.Find((x) => x.betId == item.betId && x.match == newitemB.match);
                    if (foundItemB != null) return true;
                }
            }
            return false;
        }
        public bool isBlackList(BetItem newitem, ref int retryCount)
        {
            bool isBlocked = false;
            try
            {
                foreach (var item in m_placedBetList)
                {
                    if (newitem.match.Equals(item.match))
                    {
                        retryCount++;
                    }

                    if (item.runnerId.Equals(newitem.runnerId))
                    {
                        isBlocked = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return isBlocked;
        }

        public bool isBlackList(BetItem newitem)
        {
            if (newitem.source == SOURCE.TIPSTER && newitem.Leader != "jlivesoccer") return false;
            try
            {
                string newFI = Utils.Between(newitem.bs, "f=", "#");

                foreach (var item in m_placedBetList)
                {
                    if (newitem.source == SOURCE.BASHING && item.source == SOURCE.BASHING) 
                    {
                        DateTime today = DateTime.Now;
                        DateTime placedDate = item.placedDate;

                        if (placedDate.Year == today.Year && placedDate.Month == today.Month && placedDate.Day == today.Day)
                        {
                            if (newitem.match == item.match)
                                return true;
                        }
                    }
                    else if(newitem.source == SOURCE.COPYBET)
                    {
                        if (item.runnerId.Equals(newitem.runnerId))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        string FI = Utils.Between(item.bs, "f=", "#");

                        if (item.runnerId.Equals(newitem.runnerId))
                        {
                            return true;
                        }
                        else if (FI == newFI)
                        {
                            return true;
                        }
                        else if (newitem.match == item.match)
                        {
                            return true;
                        }
                    }
                    
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool isBlackList(ref BetItem newitem, bool bWin = false)
        {
            try
            {
                int placedCount = 0;

                for (int i = 0; i < m_placedBetList.Count; i++)
                {
                    BetItem item = m_placedBetList[m_placedBetList.Count - 1 - i];
                    try
                    {
                        if (string.IsNullOrEmpty(item.pick)) continue;
                        bool isSameMatch = item.match.Equals(newitem.match);
                        bool isSamePick = item.pick.Equals(newitem.pick);
                        bool isSameRunnerId = item.runnerId.Equals(newitem.runnerId);
                        bool isSameOdds = Math.Abs(item.odds - newitem.odds) < 0.01;
                        double valueDiff = newitem.oddsDistance - item.oddsDistance;
                        if (isSameRunnerId)
                        {
                            placedCount++;
                            if (isSameOdds && Math.Abs(valueDiff) < 0.01)
                            {
                                return true;
                            }
                            else if (valueDiff < 10)
                            {
                                return true;
                            }
                            else if (newitem.odds > 2.8 && !bWin)
                            {
                                return true;
                            }
                        }
                        else if (isSameMatch)
                        {
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (placedCount > 1)
                    return true;
                else if (placedCount > 0)
                {
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in isBlackList " + ex.ToString());
            }
            return false;
        }

        public void addTriedList(BetItem betitem)
        {
            foreach (var item in m_oddsChangedBetList)
            {
                if (item.runnerId.Equals(betitem.runnerId))
                {
                    item.timestamp = betitem.timestamp;
                    item.retryCount++;
                    return;
                }
            }
            m_oddsChangedBetList.Add(betitem);
        }

        public double getElapsedSeconds(BetItem betitem)
        {
            double elapsed = 100000;
            foreach (var item in m_oddsChangedBetList)
            {
                if (item.runnerId.Equals(betitem.runnerId))
                {
                    if((DateTime.Now - item.timestamp).TotalSeconds < elapsed)
                    {
                        elapsed = (DateTime.Now - item.timestamp).TotalSeconds;
                    }
                }
            }
            return elapsed;
        }

        public bool isOddsChanged(BetItem newitem)
        {
            try
            {
                //if (newitem.source != SOURCE.BETBURGER && newitem.source != SOURCE.BASHING && newitem.source != SOURCE.DOGWIN) return false;
                int triedCount = 0;
                BetItem lastedItem = null;
                foreach (var item in m_oddsChangedBetList)
                {
                    if (item.runnerId.Equals(newitem.runnerId))
                    {
                        lastedItem = item;
                        triedCount++;
                    }
                }
                if (lastedItem == null) return false;
                //
                if (lastedItem.newOdds != newitem.odds) return true;
                double diffSeconds = Math.Abs((DateTime.Now - lastedItem.timestamp).TotalSeconds);
                if(newitem.source != SOURCE.DOGWIN)
                {
                    if (lastedItem.retryCount >= 15) return true;
                    if (diffSeconds < 60 * 2) return true;
                    if (lastedItem.bSuspended) return true;
                }
                else
                {
                    if (lastedItem.retryCount >= 6) return true;
                    if (diffSeconds < 30) return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public int getRetriedCount(BetItem newitem)
        {
            int totalRetried = 0;
            try
            {
                foreach (var item in m_oddsChangedBetList)
                {
                    if (item.runnerId.Equals(newitem.runnerId))
                    {
                        if (item.retryCount <= 0) item.retryCount = 1;
                        totalRetried += item.retryCount;
                    }
                }
            }
            catch (Exception)
            {
            }
            return totalRetried;
        }

        async public void startListening()
        {
            Setting setting = Setting.instance;
            if (_socket != null)
            {
                try
                {
                    await _socket.DisconnectAsync();
                    _socket = null;
                    m_pingThread.Abort();
                }
                catch
                {
                }
            }

            _socket  = new SocketIoClient();
            if(m_pingThread!=null) m_pingThread.Abort();
            m_pingThread = new Thread(pingThreadFunc);
            m_pingThread.Start();

            _socket.Connected += async (sender, e) =>
            {
                if (GlobalConstants.validationState != ValidationState.SUCCESS)
                {
                    SendPresentInfo(-1);
                }
                if(_reconnectThread != null) _reconnectThread.Abort();
                m_handlerWriteStatus("Socket is connnected");
                Setting.instance.isOnline = true;
                m_handlerProcUpdateNetworkStatus(true);
            };

            _socket.Disconnected += async (sender, e) =>
            {
                try
                {
                    m_handlerWriteStatus("Socket is closed: " + e.Reason.ToString());
                    _reconnectThread = new Thread(async () =>
                    {
                        Thread.Sleep(1000 * 2);
                        await _socket.ConnectAsync(new Uri(Setting.instance.serverAddr));
                    });
                    Setting.instance.isOnline = false;
                    m_handlerProcUpdateNetworkStatus(false);
                }
                catch
                {
                }
            };

            _socket.ErrorReceived += async(sender, e) =>
            {
                Setting.instance.isOnline = false;
                m_handlerProcUpdateNetworkStatus(false);
            };

            _socket.On("currentPWVersion", (data) => {
                Version localVersion = new Version(Setting.instance.version);
                Version remoteVersion = new Version(data.ToString());
                if (remoteVersion != localVersion)
                {
                    StartBetUpdater();
                    Environment.Exit(-1);
                }
            });

            _socket.On("taskScrapTokens", (payload) =>
            {
                m_handlerWriteStatus(payload.ToString());
            });

            _socket.On("botConfig", (data) => {
                try
                {
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                    string userName = jsonData.username.ToString();
                    if (userName.ToLower() != Setting.instance.betUsername.ToLower()) return;
                    Setting.instance.BotSetting = JsonConvert.DeserializeObject<dynamic>(jsonData.botConfig.ToString());
                }
                catch
                {
                }
            });

            _socket.On("restart", (data) => {
                try
                {
                    string strMessage = data.ToString();
                    if (strMessage.ToLower() == setting.betUsername.ToLower() || strMessage == "all")
                    {
                        Setting.instance.WriteRegistry("bAutoStart", "1");
                        Process.Start(Application.ExecutablePath);
                        Process.GetCurrentProcess().Kill();
                    }
                }
                catch
                {

                }
            });

            _socket.On("refreshBalance", (data) => {
                try
                {
                    string strMessage = data.ToString();
                    if (strMessage == setting.betUsername || strMessage == "all")
                    {
                        m_handlerWriteStatus("refreshBalance");
                        Task.Run(async () =>
                        {
                            double balance = BetUIController.Intance.GetBalance();
                            m_handlerWriteStatus(string.Format("Current Balance: {0}", balance));
                            if (balance > 0)
                            {
                                dynamic payload = new JObject();
                                payload.username = Setting.instance.betUsername;
                                payload.balance = balance;
                                payload.openbetAmount = 0;
                                payload.openbetCount = 0;
                                SendData("updateBalance", payload);
                            }
                            if (Setting.instance.isRecordResult)
                            {
                                JArray jsonData = BetUIController.Intance.getSettledBets();
                                CustomEndpoint.postRequest("http://92.119.237.234:5002/history/updateBbBet", jsonData.ToString());
                            }
                        });
                    }
                }
                catch
                {

                }
            });

            _socket.On("oddscpSuperbetFeeds", async (data) =>
            {
                //m_handlerWriteStatus(data.ToString());
                //JObject socketData = JObject.Parse(data.ToString());
                var ret = await sendApiWithDolpin(data);
            });

            _socket.On("validate_result", (data) =>
            {
                try
                {
                    m_handlerWriteStatus("validate_result => " + data.ToString());
                    switch (int.Parse(data.ToString()))
                    {
                        case (int)VALIDATE_CODE.SUCCESS:
                            GlobalConstants.validationState = ValidationState.SUCCESS;
                            m_handlerWriteStatus("Your key has been validated.");
                            break;
                        case (int)VALIDATE_CODE.NOEXIST_KEY:
                            GlobalConstants.state = State.Pause;
                            GlobalConstants.validationState = ValidationState.FAILURE;
                            m_handlerWriteStatus("Key is incorrect.");
                            break;
                        case (int)VALIDATE_CODE.NEW_KEY:
                            GlobalConstants.state = State.Pause;
                            GlobalConstants.validationState = ValidationState.FAILURE;
                            m_handlerWriteStatus(">>Please ask support to get license key!!");
                            break;
                        case (int)VALIDATE_CODE.INUSE_KEY:
                            GlobalConstants.state = State.Pause;
                            GlobalConstants.validationState = ValidationState.FAILURE;
                            m_handlerWriteStatus("Key is already in use.");
                            break;
                        case (int)VALIDATE_CODE.PAUSED_KEY:
                            GlobalConstants.state = State.Pause;
                            GlobalConstants.validationState = ValidationState.FAILURE;
                            m_handlerWriteStatus("Key is currently suspended, please contact support.");
                            break;
                        default:
                            GlobalConstants.state = State.Pause;
                            GlobalConstants.validationState = ValidationState.FAILURE;
                            m_handlerWriteStatus("Unknown validation code received!!!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in validate_result message: " + ex.ToString());
                }
            });

            _socket.On("betburger_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (!GetBoolVal("betburger.horse.enabled") && !GetBoolVal("betburger.sport.enabled")) return;
                    m_handlerWriteStatus(data.ToString());
                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem[] betitems = JsonConvert.DeserializeObject<BetItem[]>(payload.bets.ToString());
                    List<BetItem> horseList = new List<BetItem>();
                    foreach (BetItem betitem in betitems)
                    {
                        if (isBlackList(betitem)) continue;
                        if (isOddsChanged(betitem)) continue;
                        if (setting.bet365Domain == "au" && betitem.eventUrl.Contains("is_live=1")) continue;

                        string setBranch = GetStringVal("betburger.branch");
                        //if (betitem.branch != setBranch && setBranch != "all") continue;
                        if (betitem.odds > GetDoubleVal("betburger.odds.max")) continue;
                        if (betitem.odds < GetDoubleVal("betburger.odds.min")) continue;

                        if (betitem.arbPercent > GetDoubleVal("betburger.value.max")) continue;
                        if (betitem.arbPercent < GetDoubleVal("betburger.value.min")) continue;

                        betitem.stake = GetDoubleVal("betburger.stake");

                        betitem.source = SOURCE.BETBURGER;
                        horseList.Add(betitem);
                    }
                    if (horseList.Count == 0) return;
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in BetslipNew message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });
            _socket.On("betslip", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    //if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem[] betitemList = JsonConvert.DeserializeObject<BetItem[]>(payload.ToString());
                    List<BetItem> horseList = new List<BetItem>();
                    foreach (var item in betitemList)
                    {
                        item.source = SOURCE.DOMBETTING;
                        item.stake = setting.numStake;
                        horseList.Add(item);
                    }
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in BetslipNew message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("dombets", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    string receivedContent = data.ToString();
                    //m_handlerWriteStatus(receivedContent);
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem[] betitems = JsonConvert.DeserializeObject<BetItem[]>(payload.bets.ToString());
                    List<BetItem> horseList = new List<BetItem>();
                    foreach (BetItem betitem in betitems)
                    {
                        int retryCount = 0;
                        bool isBlocked = isBlackList(betitem, ref retryCount);
                        if (isBlocked) continue;

                        if (GetBoolVal("dombetting.enabled") != true) continue;
                        if (betitem.odds > GetDoubleVal("dombetting.odds.max")) continue;
                        if (betitem.odds < GetDoubleVal("dombetting.odds.min")) continue;

                        if (betitem.arbPercent > GetDoubleVal("dombetting.value.max")) continue;
                        if (betitem.arbPercent < GetDoubleVal("dombetting.value.min")) continue;
                        betitem.stake = GetDoubleVal("dombetting.stake");

                        betitem.source = SOURCE.DOMBETTING;
                        horseList.Add(betitem);
                    }
                    if (horseList.Count == 0) return;
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in BetslipNew message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("betburger_tips1", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    string receivedContent = data.ToString();
                    //m_handlerWriteStatus(receivedContent);
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem[] betitems = JsonConvert.DeserializeObject<BetItem[]>(payload.bets.ToString());
                    List<BetItem> horseList = new List<BetItem>();
                    foreach (BetItem betitem in betitems)
                    {
                        int retryCount = 0;
                        bool isBlocked = isBlackList(betitem, ref retryCount);
                        if (isBlocked) continue;
                        //if (isOddsChanged(betitem)) continue;

                        if (setting.bet365Domain == "au" && betitem.eventUrl.Contains("is_live=1")) continue;
                        if (betitem.sport == "Horse Racing")
                        {
                            if (GetBoolVal("betburger.horse.enabled") != true) continue;
                            if (betitem.odds < GetDoubleVal("betburger.horse.odds.win")) continue;
                            if (betitem.arbPercent < GetDoubleVal("betburger.horse.value.ew")) continue;

                            bool bEW = false;
                            if (betitem.odds >= GetDoubleVal("betburger.horse.odds.ew")) bEW = true;

                            if (!bEW && betitem.arbPercent < GetDoubleVal("betburger.horse.value.win")) continue;

                            betitem.bEW = bEW;
                            if(bEW) betitem.stake = GetDoubleVal("betburger.horse.stake.ew");
                            else betitem.stake = GetDoubleVal("betburger.horse.stake.win");

                            string setBranch = GetStringVal("betburger.horse.branch");
                            if (string.IsNullOrEmpty(setBranch)) setBranch = "all";
                            if (betitem.branch != setBranch && setBranch != "all") continue;
                        }
                        else
                        {
                            if (GetBoolVal("betburger.sport.enabled") != true) continue;
                            if (betitem.odds > GetDoubleVal("betburger.sport.odds.max")) continue;
                            if (betitem.odds < GetDoubleVal("betburger.sport.odds.min")) continue;

                            if (betitem.arbPercent > GetDoubleVal("betburger.sport.value.max")) continue;
                            if (betitem.arbPercent < GetDoubleVal("betburger.sport.value.min")) continue;
                            betitem.stake = GetDoubleVal("betburger.sport.stake");
                            if (retryCount == 0)
                                betitem.stake = GetDoubleVal("betburger.sport.stake");
                            else if (retryCount == 1)
                                betitem.stake = GetDoubleVal("betburger.sport.stake2");
                            else if (retryCount == 2)
                                betitem.stake = GetDoubleVal("betburger.sport.stake3");
                            else if (retryCount == 3)
                                betitem.stake = GetDoubleVal("betburger.sport.stake4");
                            else
                                continue;

                            string setBranch = GetStringVal("betburger.sport.branch");
                            if (string.IsNullOrEmpty(setBranch)) setBranch = "all";
                            if (betitem.branch != setBranch && setBranch != "all") continue;
                        }
                        
                        betitem.source = SOURCE.BETBURGER;
                        horseList.Add(betitem);
                    }
                    if (horseList.Count == 0) return;
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in BetslipNew message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("livebf_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    string receivedContent = data.ToString();
                    BetItem[] betitems = JsonConvert.DeserializeObject<BetItem[]>(receivedContent);
                    List<BetItem> betList = new List<BetItem>();
                    foreach (BetItem betitem in betitems)
                    {
                        int retryCount = 0;
                        bool isBlocked = isBlackList(betitem, ref retryCount);
                        if (isBlocked) continue;

                        if (GetBoolVal("live.bf.enabled") != true) continue;
                        if (betitem.odds > GetDoubleVal("live.bf.odds.max")) continue;
                        if (betitem.odds < GetDoubleVal("live.bf.odds.min")) continue;

                        if (betitem.arbPercent > GetDoubleVal("live.bf.value.max")) continue;
                        if (betitem.arbPercent < GetDoubleVal("live.bf.value.min")) continue;
                        betitem.stake = GetDoubleVal("live.bf.stake");
                        if (retryCount == 0)
                            betitem.stake = GetDoubleVal("live.bf.stake");
                        else if (retryCount == 1)
                            betitem.stake = GetDoubleVal("live.bf.stake2");
                        else if (retryCount == 2)
                            betitem.stake = GetDoubleVal("live.bf.stake3");
                        else if (retryCount == 3)
                            betitem.stake = GetDoubleVal("live.bf.stake4");
                        else
                            continue;

                        betitem.source = SOURCE.BFLIVE;
                        betList.Add(betitem);
                    }
                    if (betList.Count == 0) return;
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in livebf_tips message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("livepin_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    string receivedContent = data.ToString();
                    BetItem[] betitems = JsonConvert.DeserializeObject<BetItem[]>(receivedContent);
                    List<BetItem> betList = new List<BetItem>();
                    foreach (BetItem betitem in betitems)
                    {
                        int retryCount = 0;
                        bool isBlocked = isBlackList(betitem, ref retryCount);
                        if (isBlocked) continue;

                        if (GetBoolVal("pinfinder.enabled") != true) continue;
                        if (betitem.odds > GetDoubleVal("pinfinder.odds.max")) continue;
                        if (betitem.odds < GetDoubleVal("pinfinder.odds.min")) continue;

                        if (betitem.arbPercent > GetDoubleVal("pinfinder.value.max")) continue;
                        if (betitem.arbPercent < GetDoubleVal("pinfinder.value.min")) continue;
                        betitem.stake = GetDoubleVal("pinfinder.stake");
                        betitem.source = SOURCE.BFLIVE;
                        betList.Add(betitem);
                    }
                    if (betList.Count == 0) return;
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in livepin_tips message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("eurobots_bet", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;

                    EuBetItem betitem = JsonConvert.DeserializeObject<EuBetItem>(data.ToString());
                    //string betexpression = string.Format("Eurobot Tip : {0} {1} {2} @{3}", betitem.tipster, betitem.match, betitem.pick, betitem.odds);
                    //m_handlerWriteStatus(betexpression);
                    BetItem betitemA = new BetItem();
                    betitemA.dbId = betitem.dbId;
                    betitemA.match = betitem.match;
                    betitemA.pick = betitem.pick;
                    betitemA.outcome = betitem.pick;
                    betitemA.odds = betitem.odds;
                    betitemA.oddsDistance = betitem.oddsDistance;
                    betitemA.arbPercent = betitem.oddsDistance;
                    betitemA.bs = betitem.bs;
                    betitemA.runnerId = betitem.runnerId;
                    betitemA.sport = betitem.sport;
                    betitemA.PD = betitem.directLink;
                    betitemA.tipster = betitem.tipster;
                    betitemA.Leader = "eurobot";

                    bool bMyTip = false;
                    EuAccount myAccount = new EuAccount(); 
                    for (int i = 0; i < betitem.userList.Count; i++)
                    {
                        EuAccount user = betitem.userList[i];
                        if (string.IsNullOrEmpty(user.b365Username) || string.IsNullOrEmpty(user.b365Password))
                            continue;
                        if (user.b365Username.ToLower().Equals(Setting.instance.betUsername.ToLower()))
                        {
                            bMyTip = true;
                            myAccount = user;
                            break;
                        }
                    }
                    if (!bMyTip) return;

                    betitemA.stake = Utils.calculateStake(betitem, myAccount);
                    betitemA.source = SOURCE.TIPSTER;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitemA);
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in tipster_bet message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("copybetMsg", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("tipster.enabled") != true) return;
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(data.ToString());
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data.ToString());

                    string strTipsterSetting = GetStringVal("tipster.leader");
                    if (!strTipsterSetting.ToLower().Contains(betitem.Leader.ToLower()) && !strTipsterSetting.Contains("all")) return;
                    
                    double stakePercent = GetDoubleVal("tipster.stakepercent");
                    double myStake = stakePercent;
                    if (GetBoolVal("tipster.stake.fixed"))
                        myStake = stakePercent;
                    else
                        myStake = betitem.stake * stakePercent / 100;

                    foreach (string tipster in strTipsterSetting.ToLower().Split(','))
                    {
                        try
                        {
                            if (tipster.Contains(betitem.Leader.ToLower()))
                            {
                                string stakePart = tipster.Split(':')[1].Trim();
                                stakePercent = Utils.ParseToDouble(stakePart.Substring(0, stakePart.Length - 1));
                                if (stakePart.EndsWith("$"))
                                    myStake = stakePercent;
                                else
                                    myStake = betitem.stake * stakePercent / 100;
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                    betitem.stake = myStake;

                    double maxStake = 0;
                    try
                    {
                        maxStake = GetDoubleVal("tipster.maxstake");
                        if (betitem.stake > maxStake && maxStake > 10) betitem.stake = maxStake;
                    }
                    catch
                    {
                    }
                    m_handlerWriteStatus(betitem.bs);
                    betitem.bEW = betitem.bs.Contains("#ew=1");
                    if(betitem.bEW == true || betitem.bs.Contains("#c=2#"))
                    {
                        double bashStake = GetDoubleVal("bash.stake");
                        betitem.stake = bashStake;
                    }

                    if(betitem.pick != "hashchange")
                        if (betitem.stake == 0) return;

                    betitem.source = SOURCE.COPYBET;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitem);
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in tipster_bet message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("tipster_bet", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("tipster.enabled") != true) return;
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(data.ToString());
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data.ToString());

                    if(betitem.match == "copybet")
                    {
                        betitem.odds  = Utils.FractionToDouble(Utils.Between(betitem.bs, "#o=", "#"));
                        betitem.stake = Utils.ParseToDouble(Utils.Between(betitem.bs, "#st=", "#"));
                    }
                    betitem.bEW = false;
                    betitem.isDouble = false;
                    try
                    {
                        if (jsonData.bEW.ToString().ToLower() == "true")
                            betitem.bEW = true;
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (jsonData.isDouble.ToString().ToLower() == "true")
                            betitem.isDouble = true;
                        betitem.selectionCount = int.Parse(jsonData.selectionCount.ToString());
                    }
                    catch
                    {
                    }
                    
                    // remove low odds part
                    //string[] bsParts = betitem.bs.Split(new string[] {"||"}, StringSplitOptions.None);
                    //List<string> newBsParts = new List<string>();
                    //foreach(string bsPart in bsParts)
                    //{
                    //    if (string.IsNullOrEmpty(bsPart)) continue;
                    //    double odds = Utils.FractionToDouble(Utils.Between(bsPart, "#o=", "#"));
                    //    if (odds < 1.1) continue;
                    //    newBsParts.Add(bsPart);
                    //}
                    //betitem.selectionCount = newBsParts.Count;
                    //if (newBsParts.Count == 1) betitem.isDouble = false;
                    //if (newBsParts.Count < 1) return;
                    //betitem.bs = String.Join("||", newBsParts.ToArray());
                    //if (!betitem.bs.EndsWith("||")) betitem.bs += "||";

                    string strTipsterSetting = GetStringVal("tipster.leader");
                    if (!strTipsterSetting.ToLower().Contains(betitem.Leader.ToLower()) && !strTipsterSetting.Contains("all")) return;
                    if (betitem.odds < GetDoubleVal("tipster.odds.min")) return;

                    double stakePercent = GetDoubleVal("tipster.stakepercent");
                    double myStake = stakePercent;
                    if (GetBoolVal("tipster.stake.fixed"))
                        myStake = stakePercent;
                    else
                        myStake = betitem.stake * stakePercent / 100;

                    foreach (string tipster in strTipsterSetting.ToLower().Split(','))
                    {
                        try
                        {
                            if (tipster.Contains(betitem.Leader.ToLower()))
                            {
                                string stakePart = tipster.Split(':')[1].Trim();
                                stakePercent = Utils.ParseToDouble(stakePart.Substring(0, stakePart.Length - 1));
                                if (stakePart.EndsWith("$"))
                                    myStake = stakePercent;
                                else
                                    myStake = betitem.stake * stakePercent / 100;
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                    betitem.stake = myStake;

                    double maxStake = 0;
                    try
                    {
                        maxStake = GetDoubleVal("tipster.maxstake");
                        if (betitem.stake > maxStake && maxStake > 10) betitem.stake = maxStake;
                    }
                    catch
                    {

                    }
                    //m_handlerWriteStatus(string.Format("Stake: {0} - max {1}", myStake, maxStake));
                    betitem.source = SOURCE.TIPSTER;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitem);
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in tipster_bet message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("dogh2h_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("dogh2h.enabled") == false && GetBoolVal("horse2h.enabled") == false) return;

                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(data.ToString());
                    betitem.isValuebet = false;
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                    betitem.runnerId = jsonData.runnerId.ToString();
                    betitem.bs = jsonData.bs.ToString();
                    betitem.arbPercent = betitem.oddsDistance;
                    betitem.odds = Math.Floor(double.Parse(jsonData.odds.ToString()) * 100) / 100;
                    if (betitem.odds < 1.45)
                        return;
                    betitem.source = SOURCE.DOG_DOG;
                    if (isBlackList(ref betitem)) return;

                    if(betitem.tipster == "horse")
                    {
                        if (GetBoolVal("horseh2h.enabled") == false) return;
                        if (betitem.oddsDistance <= GetDoubleVal("horseh2h.value.h2h")) return;
                        betitem.stake = GetDoubleVal("horseh2h.stake");
                        if (betitem.leftSeconds >= GetDoubleVal("horseh2h.kickoff")) return;

                        if (betitem.pick.StartsWith("B Beat to"))
                        {
                            if (betitem.winB < GetDoubleVal("horseh2h.value.win")) return;
                        }
                        else
                        {
                            if (betitem.winA < GetDoubleVal("horseh2h.value.win")) return;
                        }
                    }
                    else if(betitem.tipster == "dog")
                    {
                        if (GetBoolVal("dogh2h.enabled") == false) return;
                        if (betitem.oddsDistance <= GetDoubleVal("dogh2h.value.h2h")) return;
                        betitem.stake = GetDoubleVal("dogh2h.stake");
                        if (betitem.leftSeconds >= GetDoubleVal("dogh2h.kickoff")) return;

                        if (betitem.pick.StartsWith("B Beat to"))
                        {
                            if (betitem.winB < GetDoubleVal("dogh2h.value.win")) return;
                        }
                        else
                        {
                            if (betitem.winA < GetDoubleVal("dogh2h.value.win")) return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    this.m_receivedBetList.Add(betitem);
                    List<BetItem> itemList = new List<BetItem>();
                    itemList.Add(betitem);
                    m_handlerProcNewtip(itemList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in dogh2h_tips message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("liverace_bets", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("liverace.horse.enabled") == false && GetBoolVal("liverace.dog.enabled") == false) return;

                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(data.ToString());
                    dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                    if (betitem.tipster == "dog")
                    {
                        if (!GetBoolVal("liverace.dog.enabled")) return;
                        if (betitem.leftSeconds > GetDoubleVal("liverace.dog.leftseconds")) return;
                        if (betitem.odds < GetDoubleVal("liverace.dog.odds.min") && betitem.odds > GetDoubleVal("liverace.dog.odds.max")) return;
                        if (betitem.oddsDistance < GetDoubleVal("liverace.dog.value.min")) return;
                        string strCountry = GetStringVal("liverace.dog.country").ToLower();
                        if (!strCountry.Contains(betitem.league.ToLower()) && strCountry != "all") return;
                        betitem.stake = GetDoubleVal("liverace.dog.stake");
                    }
                    else if (betitem.tipster == "horse")
                    {
                        if (!GetBoolVal("liverace.horse.enabled")) return;
                        if (betitem.leftSeconds > GetDoubleVal("liverace.horse.leftseconds")) return;
                        if (betitem.odds < GetDoubleVal("liverace.horse.odds.min") && betitem.odds > GetDoubleVal("liverace.horse.odds.max")) return;
                        if (betitem.oddsDistance < GetDoubleVal("liverace.horse.value.min")) return;
                        string strCountry = GetStringVal("liverace.horse.country").ToLower();
                        if (!strCountry.Contains(betitem.league.ToLower()) && strCountry != "all") return;
                        betitem.stake = GetDoubleVal("liverace.horse.stake");
                    }
                    else
                    {
                        return;
                    }

                    betitem.runnerId = jsonData.runnerId.ToString();
                    betitem.bs = jsonData.bs.ToString();
                    betitem.source = SOURCE.DOGWIN;

                    if (isBlackList(betitem)) return;
                    if (isOddsChanged(betitem)) return;
                    
                    this.m_receivedBetList.Add(betitem);
                    List<BetItem> itemList = new List<BetItem>();
                    itemList.Add(betitem);
                    m_handlerProcNewtip(itemList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in BetslipNew message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("racinginvest_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("racinginvest.enabled") != true) return;

                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(receivedContent);

                    if (isBlackList(betitem)) return;


                    if (betitem.odds < GetDoubleVal("racinginvest.odds.min")) return;

                    double stakePercent = GetDoubleVal("racinginvest.stakepercent");
                    double myStake = stakePercent;
                    if (GetBoolVal("racinginvest.stake.fixed"))
                        myStake = stakePercent;
                    else
                        myStake = betitem.stake * stakePercent / 100;
                    betitem.stake = myStake;
                    betitem.source = SOURCE.RACING_INVEST;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitem);
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in win365bet message: " + ex.ToString());
                    //m_handlerWriteStatus(Utils.DecryptMessage(data.ToString(), _KEY, _IV));
                }
            });

            _socket.On("usahorse_tips", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("usahorse.enabled") != true) return;

                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    string userTerm = payload.userTerm.ToString();
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(receivedContent);

                    if (isBlackList(betitem)) return;


                    if (betitem.odds > GetDoubleVal("usahorse.odds.max")) return;
                    if (betitem.odds < GetDoubleVal("usahorse.odds.min")) return;

                    if (betitem.value > GetDoubleVal("usahorse.value.max")) return;
                    if (betitem.value < GetDoubleVal("usahorse.value.min")) return;


                    //betitem.value = betitem.oddsDistance;
                    betitem.userTerm = userTerm;
                    betitem.stake = GetDoubleVal("usahorse.stake");

                    betitem.source = SOURCE.USAHORSE;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitem);
                    m_handlerProcNewtip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in win365bet message: " + ex.ToString());
                    //m_handlerWriteStatus(Utils.DecryptMessage(data.ToString(), _KEY, _IV));
                }
            });

            _socket.On("trademateFeeds", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("trademate.enabled") != true) return;
                    //if (countOfTodayBashBet > 15) return;

                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(receivedContent);
                    betitem.retryCount = 0;
                    string betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}%", betitem.sport, betitem.match, betitem.outcome, betitem.odds, betitem.arbPercent);
                    if (GetBoolVal($"trademate.{betitem.sport}.enabled") != true) 
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : {betitem.sport} is Not Enabled.");
                        return;
                    } 
                    betitem.source = SOURCE.TRADEMATE;
                    
                    //if (isOddsChanged(betitem)) return;

                    int retryCount = 0;
                    bool isBlocked = isBlackList(betitem, ref retryCount);
                    if (isBlocked) 
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : It is already Placed.");
                        return;
                    }

                    if (retryCount >= GetDoubleVal("trademate.betcount.event")) 
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : {retryCount} bets for this Event is Placed.");
                        return;

                    }

                    if (betitem.beforeKickOff >= GetDoubleVal("trademate.before.minute")) 
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : beforeKickOff: {betitem.beforeKickOff} > trademate.before.minute : {GetDoubleVal("trademate.before.minute")}");
                        return;
                    }

                    if (getElapsedSeconds(betitem) < GetDoubleVal("trademate.retry.delay"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : elspased: {getElapsedSeconds(betitem)} < trademate.retry.delay : {GetDoubleVal("trademate.retry.delay")}");
                        return;
                    }

                    if (getRetriedCount(betitem) >= (int)GetDoubleVal("trademate.retry.count"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : trademate.retry.count: {GetDoubleVal("trademate.retry.count")}");
                        return;
                    }

                    betitem.oddsDistance = GetDoubleVal("trademate.oddsdrop.min");

                    if (betitem.odds > GetDoubleVal("trademate.odds.max") || betitem.odds < GetDoubleVal("trademate.odds.min")) 
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : odds: {betitem.odds} is out of range.");
                        return;
                    } 

                    if (betitem.arbPercent > GetDoubleVal("trademate.value.max") || betitem.arbPercent < GetDoubleVal("trademate.value.min"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : value: {betitem.arbPercent} is out of range.");
                        return;
                    }
                    betitem.tipster = "trademate";
                    betitem.userTerm = string.Empty;
                    betitem.stake = GetDoubleVal("trademate.stake");
                    if (setting.numStake > 0) betitem.stake = setting.numStake;
                    
                    List<BetItem> horseList = new List<BetItem>();
                    horseList.Add(betitem);
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in bash_tips message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            _socket.On("rebelFeeds", (data) =>
            {
                try
                {
                    //If it is not running, all tips are skipped!!!
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    //if (countOfTodayBashBet > 15) return;

                    string receivedContent = data.ToString();
                    dynamic payload = JsonConvert.DeserializeObject<dynamic>(receivedContent);
                    BetItem betitem = JsonConvert.DeserializeObject<BetItem>(receivedContent);
                    betitem.retryCount = 0;
                    string tst = betitem.tipster;
                    if (GetBoolVal($"{tst}.enabled") != true) return;
                    string betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}%", betitem.sport, betitem.match, betitem.outcome, betitem.odds, betitem.arbPercent);
                    if (GetBoolVal($"{tst}.{betitem.sport}.enabled") != true)
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : {betitem.sport} is Not Enabled.");
                        return;
                    }
                    betitem.source = SOURCE.TRADEMATE;

                    //if (isOddsChanged(betitem)) return;

                    int retryCount = 0;
                    bool isBlocked = isBlackList(betitem, ref retryCount);
                    if (isBlocked)
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : It is already Placed.");
                        return;
                    }

                    if (retryCount >= GetDoubleVal($"{tst}.betcount.event"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : {retryCount} bets for this Event is Placed.");
                        return;

                    }

                    if (betitem.beforeKickOff >= GetDoubleVal($"{tst}.before.minute"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : beforeKickOff: {betitem.beforeKickOff} > trademate.before.minute : {GetDoubleVal("trademate.before.minute")}");
                        return;
                    }

                    if (getElapsedSeconds(betitem) < GetDoubleVal($"{tst}.retry.delay"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : elspased: {getElapsedSeconds(betitem)} < trademate.retry.delay : {GetDoubleVal("trademate.retry.delay")}");
                        return;
                    }

                    if (getRetriedCount(betitem) >= (int)GetDoubleVal($"{tst}.retry.count"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : trademate.retry.count: {GetDoubleVal("trademate.retry.count")}");
                        return;
                    }

                    betitem.oddsDistance = GetDoubleVal($"{tst}.oddsdrop.min");

                    if (betitem.odds > GetDoubleVal($"{tst}.odds.max") || betitem.odds < GetDoubleVal($"{tst}.odds.min"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : odds: {betitem.odds} is out of range.");
                        return;
                    }
                    if (betitem.isLive && !GetBoolVal(tst + ".live"))
                    {
                        //mainActivity.OnWriteLog(tipster + ".value.min");
                        return;
                    }

                    if (betitem.arbPercent > GetDoubleVal($"{tst}.value.max") || betitem.arbPercent < GetDoubleVal($"{tst}.value.min"))
                    {
                        //m_handlerWriteStatus($"{betexpression} - SKIP : value: {betitem.arbPercent} is out of range.");
                        return;
                    }
                    if (betitem.isLive && !GetBoolVal(tst +  ".live"))
                    {
                        //mainActivity.OnWriteLog(tipster + ".value.min");
                        return;
                    }
                    if (!betitem.isLive && !GetBoolVal(tst + ".prematch"))
                    {
                        //mainActivity.OnWriteLog(tipster + ".value.min");
                        return;
                    }
                    betitem.userTerm = string.Empty;
                    betitem.stake = GetDoubleVal($"{tst}.stake");
                    if (setting.numStake > 0) betitem.stake = setting.numStake;

                    List<BetItem> horseList = new List<BetItem>();
                    horseList.Add(betitem);
                    m_handlerProcNewtip(horseList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in bash_tips message: " + ex.ToString());
                    m_handlerWriteStatus(data.ToString());
                }
            });

            await _socket.ConnectAsync(new Uri(Setting.instance.serverAddr));
        }

        public void StartBetUpdater()
        {
            try
            {
                Thread.Sleep(1000 * 10);
                Setting.instance.WriteRegistry("bAutoStart", "1");
                ProcessStartInfo startInfo = new ProcessStartInfo("BetLauncher.exe");
                Process checkerProcess = new Process();
                checkerProcess.StartInfo = startInfo;
                checkerProcess.EnableRaisingEvents = true;
                checkerProcess.Start();
            }
            catch
            {
            }
        }


        public string GetStringVal(string keyName)
        {
            try
            {
                string ketValue = Setting.instance.BotSetting[keyName].ToString();
                return ketValue;
            }
            catch
            {
            }
            return string.Empty;
        }

        public double GetDoubleVal(string keyName)
        {
            try
            {
                string ketValue = Setting.instance.BotSetting[keyName].ToString();
                return Utils.ParseToDouble(ketValue);
            }
            catch
            {
            }
            return -1;
        }

        public bool GetBoolVal(string keyName)
        {
            try
            {
                return Setting.instance.BotSetting[keyName] == true;
            }
            catch
            {
            }
            return false;
        }

        public void SendData(string messageCode, JObject payload)
        {
            try
            {
                if (_socket == null) return;
                _socket.Emit(messageCode, payload);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in SocketConnector.SendData message: " + ex.ToString());
            }
        }

        public void SendPresentInfo(double curbalance)
        {
            if(GlobalConstants.validationState == ValidationState.WITHOUT_KEY)
            {
                return;
            }
            dynamic payload = new JObject();
            payload.username = Setting.instance.betUsername;
            payload.password = Setting.instance.betPassword;
            payload.country = Setting.instance.countryCode;
            payload.owner   = Setting.instance.owner;
            payload.balance = curbalance;
            payload.version = Setting.instance.version;
            payload.license = Setting.instance.license;
            payload.anydesk = Setting.instance.anydesk;
            //m_handlerWriteStatus(payload.ToString());
            _socket.Emit("present_bot", payload);
        }

        public async void dolphinAPi()
        {
            string dolphinUrl = Setting.instance.dolphinUrl;
            string token = Setting.instance.dolphinToken;
            string profileId = Setting.instance.dolphinProfileId;
            try
            {
                if (string.IsNullOrWhiteSpace(dolphinUrl))
                {
                    m_handlerWriteStatus("Dolphin URL cannot be empty.");
                    return;
                }
                string url = dolphinUrl + "/auth/login-with-token";
                var jsonData = new
                {
                    token
                };
                string json = JsonConvert.SerializeObject(jsonData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                m_handlerWriteStatus("You have successfully logged into Dolphin.");
            }
            catch (HttpRequestException httpEx)
            {
                m_handlerWriteStatus(httpEx.Message);
            }
            catch (JsonException jsonEx)
            {
                m_handlerWriteStatus($"JSON Error: {jsonEx.Message}");
                Console.Error.WriteLine(jsonEx);
            }
            try
            {
                m_handlerWriteStatus("Waiting for connecting to Dolphin profile");
                string url = dolphinUrl + "/browser_profiles/" + profileId + "/start?automation=1";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

#pragma warning disable CS8600
                JObject jObject = JObject.Parse(responseBody);
                var automation = jObject["automation"];
                var wsEndpoint = automation["wsEndpoint"];
                var port = automation["port"];
                m_handlerWriteStatus(wsEndpoint + ":" + port);

                var puppeteer = new PuppeteerExtra();
                var stealth = new StealthPlugin();
                puppeteer.Use(stealth);
                var browser = await puppeteer.ConnectAsync(new ConnectOptions
                {
                    BrowserWSEndpoint = "ws://127.0.0.1:" + port + wsEndpoint
                }
                );
                page = await browser.NewPageAsync();
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1480,
                    Height = 720
                });
                m_handlerWriteStatus("Superbet.com page is loading...");
                await page.GoToAsync("https://superbet.com", new NavigationOptions()
                {
                    Timeout = 0
                });
                await page.WaitForNetworkIdleAsync();
                m_handlerWriteStatus("Page successufully loaded");

                var timeOut = await page.QuerySelectorAsync("div[data-v-292d173a]");
                if (timeOut != null)
                {
                    await page.ClickAsync("button[data-v-46774afa]");
                    m_handlerWriteStatus("Timout button clicked");
                }
                var cookiePanel = await page.QuerySelectorAsync("div[id=\'onetrust-banner-sdk\']");
                if (cookiePanel != null)
                {
                    await page.ClickAsync("button[id=\"onetrust-accept-btn-handler\"]");
                    m_handlerWriteStatus("Accept Cookies button clicked");
                }

                await page.ClickAsync("button.e2e-login");
                await page.WaitForSelectorAsync("div.login-modal");
                m_handlerWriteStatus("Login button clicked");
                var nameField = await page.QuerySelectorAsync("div.e2e-login-username input");
                await nameField.TypeAsync(Setting.instance.betUsername);
                var passwordField = await page.QuerySelectorAsync("div.e2e-login-password input");
                await passwordField.TypeAsync(Setting.instance.betPassword);
                await page.ClickAsync("button.e2e-login-submit-btn");
                m_handlerWriteStatus("Login button clicked");
                await page.WaitForSelectorAsync("div[data-v-8d3cc814]", new WaitForSelectorOptions { Timeout = 60000 });
                var bonusPanel = await page.QuerySelectorAsync("div[data-v-292d173a]");
                if (bonusPanel != null)
                {
                    await page.ClickAsync("button[data-v-4500df5e]");
                    m_handlerWriteStatus("Bonus button clicked");
                }
                m_handlerWriteStatus("Successfully logged in");
                var userJson = await page.EvaluateFunctionAsync(@"
                    key => {
                        try{
                            return localStorage.getItem(key);
                        }catch(error){
                            return null;
                        }    
                    }
                ", "user");
                JObject user = JObject.Parse(userJson.ToString());
                var value = user["value"];
                var balance = value["balance"];
                var realMoney = balance["realMoney"];
                var total = realMoney["total"];
                this.balance = (float)total;
                m_handlerWriteStatus("balance: " + total.ToString() + "$");

            }
            catch (HttpRequestException httpEx)
            {
                m_handlerWriteStatus(httpEx.Message);
            }
        }
        public async Task<bool> sendApiWithDolpin(string socketData)
        {
            JArray obj = JArray.Parse(socketData);
            var url = obj[0]["directLink"].ToString();
            string pattern = @"(\d+)(?=\?t=)";
            Match match = Regex.Match(url, pattern);
            if (match.Success)
            {
                // Extract the number from the first capture group
                string extractedNumber = match.Groups[1].Value;
                m_handlerWriteStatus(extractedNumber); // Outputs: 7042833
                HttpClient client = new HttpClient();
                var durl = "https://production-superbet-offer-basic.freetls.fastly.net/sb-basic/api/v2/en-BR/events/" + extractedNumber + "?matchIds=" + extractedNumber;
                HttpResponseMessage response = await client.GetAsync(durl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject all = JObject.Parse(responseBody);
                JObject eventData = (JObject)all["data"].First;
                JObject ticketData = (JObject)eventData["odds"]?.First;

                if (ticketData == null)
                    return false;
                Ticket ticket = new Ticket();

                ticket.ticketOnline = "online";
                ticket.total = 1;
                ticket.betType = "prematch";
                ticket.combs = "";
                ticket.clientSourceType = "Desktop_new";
                ticket.paymentBonusType = 1;
                ticket.locale = "pt-BR";
                ticket.deviceIdentifier = "1ab9511b-adfc-40ef-b960-6c6989c9094f";
                ticket.autoAcceptChanges = "1";
                ticket.ticketUuid = obj[0]["runnerId"].ToString();
                RequestDetail rd = new RequestDetail();
                rd.deviceId = "1ab9511b-adfc-40ef-b960-6c6989c9094f";
                rd.ldAnonymousUserKey = "ANONYMOUS_USER-841";
                rd.isDeviceIdTestFlagOnSubscribed = "false";
                rd.isDeviceIdTestFlagOnInitial = "false";
                ticket.requestDetails = rd;
                TicketBetItem ticketBetItem = new TicketBetItem();
                ticketBetItem.matchId = int.Parse(extractedNumber);
                ticketBetItem.value = (string)ticketData["price"];
                ticketBetItem.matchName = (string)eventData["matchName"];
                ticketBetItem.oddFullName = (string)ticketData["name"];
                ticketBetItem.matchDate = (string)eventData["matchDate"];
                ticketBetItem.matchDateUtc = (string)eventData["utcDate"];
                ticketBetItem.selected = true;
                ticketBetItem.fix = false;
                string[] teams = ticketBetItem.matchName.Split('·');
                ticketBetItem.teamnameone = teams[0];
                ticketBetItem.teamnametwo = teams[1];
                ticketBetItem.teamId1 = (string)eventData["homeTeamId"];
                ticketBetItem.teamId2 = (string)eventData["awayTeamId"];
                ticketBetItem.tournamentId = (int)eventData["tournamentId"];
                ticketBetItem.oddDescription = (string)ticketData["info"];
                ticketBetItem.sportName = (string)obj[0]["sport"];
                ticketBetItem.sportId = (int)eventData["sportId"];
                ticketBetItem.live = (bool)eventData["hasLive"];
                ticketBetItem.eventId = int.Parse(extractedNumber);
                ticketBetItem.eventUuid = (string)eventData["uuid"];
                ticketBetItem.eventName = (string)eventData["matchName"];
                ticketBetItem.eventCode = (int)eventData["matchCode"];
                ticketBetItem.marketId = (int)ticketData["marketId"];
                ticketBetItem.marketUuid = (string)ticketData["marketUuid"];
                ticketBetItem.oddId = (int)ticketData["outcomeId"];
                ticketBetItem.oddName = (string)ticketData["name"];
                ticketBetItem.oddCode = (string)ticketData["code"];
                ticketBetItem.uuid = (string)eventData["uuid"];
                ticketBetItem.oddUuid = (string)ticketData["uuid"];
                ticket.items = new TicketBetItem[] { ticketBetItem};
                //m_handlerWriteStatus(ticketBetItem.ToString());

//                string dolphinUrl = Setting.instance.dolphinUrl;
//                string token = Setting.instance.dolphinToken;
//                string profileId = Setting.instance.dolphinProfileId;
//                try
//                {
//                    if (string.IsNullOrWhiteSpace(dolphinUrl))
//                    {
//                        m_handlerWriteStatus("Dolphin URL cannot be empty.");
//                    }
//                    string urld = dolphinUrl + "/auth/login-with-token";
//                    var jsonData = new
//                    {
//                        token
//                    };
//                    string json = JsonConvert.SerializeObject(jsonData);
//                    var content = new StringContent(json, Encoding.UTF8, "application/json");
//                    HttpResponseMessage responsed = await client.PostAsync(urld, content);
//                    responsed.EnsureSuccessStatusCode();

//                    string responseBodyd = await responsed.Content.ReadAsStringAsync();
//                    m_handlerWriteStatus("You have successfully logged into Dolphin.");
//                }
//                catch (HttpRequestException httpEx)
//                {
//                    m_handlerWriteStatus(httpEx.Message);
//                }
//                catch (JsonException jsonEx)
//                {
//                    m_handlerWriteStatus($"JSON Error: {jsonEx.Message}");
//                    Console.Error.WriteLine(jsonEx);
//                }
//                try
//                {
//                    m_handlerWriteStatus("Waiting for connecting to Dolphin profile");
//                    string urlp = dolphinUrl + "/browser_profiles/" + profileId + "/start?automation=1";
//                    HttpResponseMessage responsep = await client.GetAsync(url);
//                    response.EnsureSuccessStatusCode();

//                    string responseBodyp = await responsep.Content.ReadAsStringAsync();

//#pragma warning disable CS8600
//                    JObject jObject = JObject.Parse(responseBodyp);
//                    var automation = jObject["automation"];
//                    var wsEndpoint = automation["wsEndpoint"];
//                    var port = automation["port"];
//                    m_handlerWriteStatus(wsEndpoint + ":" + port);

//                    var puppeteer = new PuppeteerExtra();
//                    var stealth = new StealthPlugin();
//                    puppeteer.Use(stealth);
//                    var browser = await puppeteer.ConnectAsync(new ConnectOptions
//                    {
//                        BrowserWSEndpoint = "ws://127.0.0.1:" + port + wsEndpoint
//                    }
//                    );
//                }catch(HttpException e)
//                {

//                }

                try
                {
                    var cookies = await page.GetCookiesAsync();
                    string cookieString = "";
                    var cookieContainer = new CookieContainer();
                    for (var i = 0; i < cookies.Length; i++)
                    {
                        var cookie = cookies[i];
                        cookieString += cookie.Name + '=' + cookie.Value + "; ";
                        cookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                    }
                    Console.WriteLine(cookieString);
                    var handler = new HttpClientHandler { CookieContainer = cookieContainer };
                    var ch = new HttpClient(handler);
                    string json = JsonConvert.SerializeObject(ticket);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    if (ticket.items.Length == 0)
                        return false;
                    var url1 = "https://api.web.production.betler.superbet.com/legacy-web/betting/submitTicket";


                    //// Set headers
                    //ch.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    //ch.DefaultRequestHeaders.Add("Accept", "application/json");
                    //ch.DefaultRequestHeaders.Add("Referer", "https://superbet.com");  // Use the URL that you're working with

                    //// Send the POST request
                    //HttpResponseMessage response1 = await ch.PostAsync(url1, content);

                    //// Inspect status code and response body
                    //if (!response1.IsSuccessStatusCode)
                    //{
                    //    string responseBody1 = await response1.Content.ReadAsStringAsync();
                    //    m_handlerWriteStatus($"Error: {response1.StatusCode}");
                    //    m_handlerWriteStatus("Response Body: " + responseBody1);
                    //}
                    //else
                    //{
                    //    // If the request was successful, process the response
                    //    string responseBody1 = await response1.Content.ReadAsStringAsync();
                    //    m_handlerWriteStatus("Response: " + responseBody1);
                    //}
                    await page.SetRequestInterceptionAsync(true);
                    if (this.balance > 1.0)
                    {
                        var responseData = await page.EvaluateFunctionAsync(@"
                        (async (cookieString, json) => {
                            const url = 'https://api.web.production.betler.superbet.com/legacy-web/betting/submitTicket?clientSourceType=Desktop_new'; // The target API endpoint
                            console.log(cookieString);
                            console.log(json);
                            try {
                                const response = await fetch(url, {
                                    'method': 'POST',
                                    'headers': {
                                        //':authority': 'api.web.production.betler.superbet.com', // Added authority header
                                        //':method': 'POST', // Added method header
                                        //':path': '/legacy-web/betting/submitTicket?clientSourceType=Desktop_new', // Added path header
                                        //':scheme': 'https', // Added scheme header
                                        'accept': 'application/json, text/plain, */*', // Accept header
                                        'accept-language': 'en-US,en;q=0.9,ko;q=0.8',
                                        'accept-encoding': 'gzip, deflate, br, zstd', // Accept-Encoding header
                                        'content-type': 'application/json', // Content-Type header
                                        'origin': 'https://superbet.com', // Origin header
                                        'cookie': cookieString, // Cookie header
                                        'referer': 'https://superbet.com/',
                                        'sec-ch-ua-mobile': '?0',
                                        'sec-ch-ua-platform': 'Windows',
                                        'sec-fetch-dest': 'empty',
                                        'sec-fetch-mode': 'cors',
                                        'sec-fetch-site': 'same-site',
                                        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36'
                                    },
                                    'referer': 'https://superbet.com/',
                                    'refererPolicy': 'strict-origin-when-cross-origin',
                                    'body': json, // Send the JSON payload in the request body
                                    'credentials': 'include',
                                    'mode': 'cors',
                                });

                                const responseData = await response.json(); // Parse JSON response
                                console.log('Response:', responseData); // Log the response
                                return responseData; // Return the response data to C#
                            } catch (error) {
                                return null; // Return null on error
                            }
                        })", cookieString, json);
                        m_handlerWriteStatus(responseData?.ToString());
                        Console.WriteLine(responseData);
                        if (responseData != null)
                        {
                            this.balance = this.balance - 1;
                        }
                        return true;
                    }
                    return false;

                }
                catch (HttpRequestException httpEx)
                {
                    m_handlerWriteStatus(httpEx.Message);
                    return false;
                }
            }
            else
            {
                m_handlerWriteStatus("No match found");
                return false;
            }
        }
            
    }
}
