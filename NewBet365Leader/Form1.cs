using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MaterialSkin;
using MaterialSkin.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using FirefoxBet365Placer.Json;
using FirefoxBet365Placer.Constants;
using FirefoxBet365Placer.Controller;
using System.Text;
using System.Net.Http;
using System.Drawing;
using Telegram.Bot;
using Newtonsoft.Json;
using System.ComponentModel;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp;
using PuppeteerSharp;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace FirefoxBet365Placer
{
    public delegate void onWriteStatusEvent(string status);
    public delegate void onProcNewTipEvent(List<BetItem> newTip);
    public delegate void onWriteLogEvent(string strLog);
    public delegate void onWriteRestartLogEvent(string strLog);
    public delegate void onProcUpdateNetworkStatusEvent(bool isOnline);

    public delegate void onProcOperationEvent(string strBody, int opCode, string betGuid);
    public delegate void onProcPlacedBetEvent(string strBody, string betGuid);

    public partial class Form1 : MaterialForm
    {
        MaterialSkin.MaterialSkinManager materialSkinManager;
        public event onProcOperationEvent onProcOperationEvent;
        public event onProcPlacedBetEvent onProcPlacedBetEvent;
        public event onProcUpdateNetworkStatusEvent onProcUpdateNetworkStatusEvent;

        public event onWriteLogEvent onWriteLog;
        public event onWriteRestartLogEvent onWriteRestartLog;
        public event onWriteStatusEvent onWriteStatus;
        public event onProcNewTipEvent onProcNewTipEvent;
        public Thread m_refreshThread;
        private SocketConnector m_socketConnector = null;
        private LiveSoccerSocktCnt m_liveDataCnt = null;

        private string m_lastToken = string.Empty;
        private string m_path;
        private List<BetItem> _betList = new List<BetItem>();
        private bool _refreshOnce;
        private DateTime _lastTriedDate;
        private HttpClient client;
        private IPage page;
        public Form1()
        {
            try
            {
                _lastTriedDate = DateTime.Now;
                InitializeComponent();
                SetColorScheme(materialSkinManager, this);
                Setting.instance.loadSettingInfo();
                this.onProcNewTipEvent += procNewTip;
                this.onProcUpdateNetworkStatusEvent += procUpdateNetworkStatus;
                this.onWriteStatus += writeStatus;
                this.onWriteLog += LogToFile;
                this.onWriteRestartLog += RestartLogToFile;
                createLogFolders();

                m_socketConnector = new SocketConnector(onWriteLog, onWriteStatus, onProcNewTipEvent);
                m_socketConnector.m_handlerProcUpdateNetworkStatus = this.onProcUpdateNetworkStatusEvent;
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    displayBets();
                });
                Setting.instance.writeRestartLog = this.onWriteRestartLog;
                this.client = new HttpClient();
            }
            catch (Exception ex)
            {
            }
        }

        public void SetColorScheme(MaterialSkin.MaterialSkinManager materialSkinManager, MaterialForm materialForm)
        {
            materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(materialForm);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            //materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey600, Primary.BlueGrey800, Primary.BlueGrey900, Accent.Green700, TextShade.WHITE);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal700, Primary.Teal300, Accent.Teal200, TextShade.WHITE);
        }
        private void createLogFolders()
        {
            m_path = Directory.GetCurrentDirectory();
            if (!Directory.Exists(m_path + "\\Log\\"))
                Directory.CreateDirectory(m_path + "\\Log\\");
            if (!Directory.Exists(m_path + "\\Bet\\"))
                Directory.CreateDirectory(m_path + "\\Bet\\");
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            this.siticoneDataGridView1.RowValidated += betDataRowValidate;
            this.siticoneDataGridView1.DataError += new DataGridViewDataErrorEventHandler(betDataError);
            this.siticoneDataGridView3.DataError += new DataGridViewDataErrorEventHandler(betDataError);
            if (Setting.instance.ReadRegistry("bAutoStart") == "1")
            {
                Thread.Sleep(1000);
                btnStart_Click(null, null);
            }
            txtUser.Text = Setting.instance.betUsername;
            CheckForIllegalCrossThreadCalls = false;
            this.Text = string.Format("BOTCHRIS v{0}", Setting.instance.version);
            txtIP.Text = Setting.instance.serverAddr;
        }

        private void betDataRowValidate(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void displayFeeds()
        {
            try
            {
                this.Invoke(new Action(() => {
                    try
                    {
                        bindingSource1.DataSource = _betList;
                        bindingSource1.ResetBindings(false);
                    }
                    catch
                    {
                    }
                    
                }));
            }
            catch { }
        }

        private void displayBets()
        {
            try
            {
                this.Invoke(new Action(() => {
                    try
                    {
                        bindingSource.DataSource = m_socketConnector.m_placedBetList;
                        bindingSource.ResetBindings(false);
                    }
                    catch
                    {

                    }
                    
                }));
                double totalStake = 0;
                int totalBet = 0;
                foreach(BetItem betitem in m_socketConnector.m_placedBetList)
                {
                    totalStake += betitem.stake;
                }
                totalBet = m_socketConnector.m_placedBetList.Count;
                this.Invoke(new Action(() => {
                    try
                    {
                        lbTurnover.Text = $" {totalBet}";
                    }
                    catch
                    {
                    }
                }));
            }
            catch { }
        }

        async public void refreshThreadFunc()
        {
            m_socketConnector.SendPresentInfo(-1);
            int retryCounter = 0;
            DateTime lastLoadingTime = DateTime.Now;
            LoadSetting();
            bool isJustFinishedSchedule = false;
            bool isStarted = true;
            displayFeeds();
            
            while (true)
            {
                try
                {
                    if(retryCounter % 50 == 0)
                    {
                        displayFeeds();
                    }
                    Thread.Sleep(100);
                    if (Setting.instance.isWorkingAtDayTime && !CheckIfWorkingFrame())
                    {
                        Thread.Sleep(1000);
                        // Need to logout here;
                        if (!isJustFinishedSchedule)
                        {
                            writeStatus("Bet365 will be logged out because schedule is over.");
                            BetController.Intance.ExecuteScript("var r = new ns_loginlib_delegates.StandardLoginWebDelegate;r.loginAPI.logout();");
                            isJustFinishedSchedule = true;
                        }
                        continue;
                    }

                    if (Setting.instance.isWorkingAtDayTime && (isJustFinishedSchedule || isStarted)) 
                    {
                        _betList.Clear();
                        writeStatus("Bet365 will be logged in because schedule is started.");
                        BetUIController.Intance.DoLogin();
                        isStarted = false;
                    }
                    isJustFinishedSchedule = false;
                    /*
                    Task.Run(() =>
                    {
                        try
                        {
                            lblBetCount.Text = $"{_betList.Count} Bets";
                        }
                        catch
                        {
                        }
                    });
                    */
                    if (retryCounter % 20 == 0 && _betList.Count == 0)
                    {
                        BetUIController.Intance.GetWidthHeight();
                        BetUIController.Intance.DoClickDlgBox();
                        FakeUserAction.Intance.taskHumanActivity();
                    }
                    retryCounter++;
                    if (Setting.instance.isDouble)
                    {
                        List<BetItem> selectedItems = new List<BetItem>();
                        lock (_betList)
                        {
                            if (_betList.Count > 1)
                            {
                                int selectedIndexA = -1, selectedIndexB = -1;
                                for(int i=0; i <  _betList.Count; i++)
                                {
                                    int retryCount = 0;
                                    BetItem itemA = _betList[i];
                                    bool isBlocked = m_socketConnector.isBlackList(itemA, ref retryCount);
                                    if (isBlocked) continue;
                                    if (retryCount >= m_socketConnector.GetDoubleVal(itemA.tipster + ".betcount.event")) continue;
                                    if (m_socketConnector.getRetriedCount(itemA) >= (int)m_socketConnector.GetDoubleVal(itemA.tipster + ".retry.count"))
                                        continue;

                                    for (int j = i+1; j < _betList.Count; j++)
                                    {
                                        BetItem itemB = _betList[j];
                                        isBlocked = m_socketConnector.isBlackList(itemB, ref retryCount);
                                        if (isBlocked) continue;
                                        if (retryCount >= m_socketConnector.GetDoubleVal(itemA.tipster + ".betcount.event")) continue;
                                        if (m_socketConnector.getRetriedCount(itemB) >= (int)m_socketConnector.GetDoubleVal(itemA.tipster + ".retry.count"))
                                            continue;

                                        if (itemA.match != itemB.match)
                                        {
                                            if (m_socketConnector.isBlackList(itemA, itemB)) continue;
                                            selectedItems.Add(itemA);
                                            selectedItems.Add(itemB);
                                            selectedIndexA = i;
                                            selectedIndexB = j;
                                            break;
                                        }
                                    }
                                    if (selectedItems.Count >= 2) break;
                                    
                                }
                                if (selectedIndexA > 0)
                                {
                                    _betList.RemoveAt(selectedIndexA);
                                }
                                if (selectedIndexB > 0)
                                {
                                    _betList.RemoveAt(selectedIndexB - 1);
                                }
                            }
                        }

                        if (selectedItems.Count > 1)
                        {
                            Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                            BetUIController.Intance.MakeBet(selectedItems);
                            displayBets();
                            foreach (BetItem betitem in selectedItems)
                            {
                                m_socketConnector.addTriedList(betitem);
                            }
                            _lastTriedDate = DateTime.Now;
                            IOManager.saveBetData(m_socketConnector.m_placedBetList);
                            Thread.Sleep(3000);
                        }
                    }
                    else
                    {
                        BetItem selectedItem = null;
                        lock (_betList)
                        {
                            if (_betList.Count > 0)
                            {
                                selectedItem = _betList[0];
                                _betList.RemoveAt(0);
                            }
                        }

                        if (selectedItem != null)
                        {
                            if (selectedItem.source == SOURCE.COPYBET)
                            {
                                if (selectedItem.pick == "login")
                                {
                                    BetUIController.Intance.DoLogin();
                                }
                                else if (selectedItem.pick == "scroll")
                                {
                                    try
                                    {
                                        Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                                        //writeStatus("scroll =>" + selectedItem.bs);

                                        JObject jsonPoint = JsonConvert.DeserializeObject<JObject>(selectedItem.bs);
                                        string width = jsonPoint.SelectToken("width").ToString();
                                        string height = jsonPoint.SelectToken("height").ToString();

                                        decimal xRatio = decimal.Parse(width) / (decimal)Setting.instance.innerWidth;
                                        decimal yRatio = decimal.Parse(height) / (decimal)Setting.instance.innerHeight;
                                        xRatio = 1 / xRatio;
                                        yRatio = 1 / yRatio;

                                        string pointerX = jsonPoint.SelectToken("pointerX").ToString();
                                        string pointerY = jsonPoint.SelectToken("pointerY").ToString();
                                        string scrollY = jsonPoint.SelectToken("scrollY").ToString();
                                        Json.Point endPos = new Json.Point(decimal.Parse(pointerX) * xRatio, decimal.Parse(pointerY) * yRatio);
                                        FakeUserAction.Intance.SimMouseMoveTo(endPos);
                                        FakeUserAction.Intance.SimWheel(decimal.Parse(scrollY) * yRatio);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else if (selectedItem.pick == "clicked")
                                {
                                    /*
                                    try
                                    {
                                        Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                                        JObject jsonPoint = JsonConvert.DeserializeObject<JObject>(selectedItem.bs);
                                        string width = jsonPoint.SelectToken("width").ToString();
                                        string height = jsonPoint.SelectToken("height").ToString();

                                        decimal xRatio = decimal.Parse(width) / (decimal)Setting.instance.innerWidth;
                                        decimal yRatio = decimal.Parse(height) / (decimal)Setting.instance.innerHeight;
                                        xRatio = 1 / xRatio;
                                        yRatio = 1 / yRatio;

                                        string clickedX = jsonPoint.SelectToken("clickedX").ToString();
                                        string clickedY = jsonPoint.SelectToken("clickedY").ToString();
                                        Json.Point endPos = new Json.Point(decimal.Parse(clickedX) * xRatio, decimal.Parse(clickedY) * yRatio);
                                        FakeUserAction.Intance.SimMouseMoveTo(endPos);
                                        FakeUserAction.Intance.SimClick();
                                    }
                                    catch
                                    {
                                    }
                                    */
                                }
                                else if (selectedItem.pick == "hashchange")
                                {
                                    if (!BetUIController.Intance.CheckIfLogged())
                                    {
                                        writeStatus("Session is logged out");
                                        if (!BetUIController.Intance.DoLogin(selectedItem.bs))
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        BetUIController.Intance.NavigateInvoke(selectedItem.bs);
                                    }
                                }
                                else if (selectedItem.pick == "addbet")
                                {
                                    if (m_socketConnector.isBlackList(selectedItem))
                                        continue;
                                    Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                                    BetUIController.Intance.Removebet();
                                    BetUIController.Intance.DoAddBetUI(selectedItem);
                                }
                                else if (selectedItem.pick == "placebet")
                                {
                                    if (m_socketConnector.isBlackList(selectedItem))
                                        continue;
                                    Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                                    string placeBetResp = BetUIController.Intance.DoPlacebetUI(selectedItem);
                                    writeStatus(placeBetResp);
                                    dynamic jsonContent = JsonConvert.DeserializeObject<dynamic>(placeBetResp);
                                    string betRespCode = jsonContent.sr.ToString();
                                    if (betRespCode == "0")
                                    {
                                        m_socketConnector.m_placedBetList.Add(selectedItem);
                                        IOManager.saveBetData(m_socketConnector.m_placedBetList);
                                    }
                                }
                                continue;
                            }
                            else if (selectedItem.source == SOURCE.TRADEMATE)
                            {
                                int retryCount = 0;
                                if (m_socketConnector.isBlackList(selectedItem, ref retryCount)) continue;
                                if (retryCount >= m_socketConnector.GetDoubleVal(selectedItem.tipster + ".betcount.event"))
                                {
                                    string betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}% - {5}", selectedItem.sport, selectedItem.match, selectedItem.outcome, selectedItem.odds, selectedItem.arbPercent, selectedItem.eventUrl);
                                    //writeStatus($"{betexpression} - SKIP : {retryCount} bets for this Event is Placed.");
                                    continue;
                                }
                                //writeStatus($"m_socketConnector.getRetriedCount(selectedItem)={m_socketConnector.getRetriedCount(selectedItem)}");
                                //writeStatus($"m_socketConnector.getElapsedSeconds(selectedItem)={m_socketConnector.getElapsedSeconds(selectedItem)}");
                                if (m_socketConnector.getRetriedCount(selectedItem) >= (int)m_socketConnector.GetDoubleVal(selectedItem.tipster + ".retry.count"))
                                {
                                    continue;
                                }

                                if (m_socketConnector.getElapsedSeconds(selectedItem) < m_socketConnector.GetDoubleVal(selectedItem.tipster + ".retry.delay"))
                                {
                                    string betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}% - {5}", selectedItem.sport, selectedItem.match, selectedItem.outcome, selectedItem.odds, selectedItem.arbPercent, selectedItem.eventUrl);
                                    //writeStatus(betexpression);
                                    //writeStatus($"SKIP : beforeKickOff: {selectedItem.beforeKickOff} > trademate.before.minute : {m_socketConnector.GetDoubleVal("trademate.before.minute")}");
                                    continue;
                                }
                            }
                            else if (selectedItem.source != SOURCE.BETBURGER &&
                                selectedItem.source != SOURCE.TIPSTER)
                            {
                                if (m_socketConnector.isBlackList(selectedItem)) continue;
                                if (m_socketConnector.isOddsChanged(selectedItem)) continue;
                            }
                            else if (selectedItem.source == SOURCE.BETBURGER)
                            {
                                int retryCount = 0;
                                bool isBlocked = m_socketConnector.isBlackList(selectedItem, ref retryCount);
                                if (isBlocked) continue;
                                selectedItem.stake = m_socketConnector.GetDoubleVal("betburger.sport.stake");
                                if (retryCount == 0)
                                    selectedItem.stake = m_socketConnector.GetDoubleVal("betburger.sport.stake");
                                else if (retryCount == 1)
                                    selectedItem.stake = m_socketConnector.GetDoubleVal("betburger.sport.stake2");
                                else if (retryCount == 2)
                                    selectedItem.stake = m_socketConnector.GetDoubleVal("betburger.sport.stake3");
                                else if (retryCount == 3)
                                    selectedItem.stake = m_socketConnector.GetDoubleVal("betburger.sport.stake4");
                                else
                                    continue;
                            }
                            if (selectedItem.stake <= 0) continue;

                            Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                            BetUIController.Intance.MakeBet(selectedItem);
                            displayBets();
                            _lastTriedDate = DateTime.Now;
                            IOManager.saveBetData(m_socketConnector.m_placedBetList);
                            selectedItem.timestamp = DateTime.Now;
                            m_socketConnector.addTriedList(selectedItem);
                            if (selectedItem.source == SOURCE.TRADEMATE)
                            {
                                Thread.Sleep(3000);
                            }
                        }
                    }

         

                    if (retryCounter > 3000)
                    {
                        BetUIController.Intance.ExecuteScript("location.reload();");
                        LoadSetting();
                        retryCounter = 0;
                    }
                }
                catch(Exception ex)
                {
                    writeStatus("Exception in refreshThreadFunc: " + ex.ToString());
                }
            }
        }

        private bool CheckIfWorkingFrame()
        {
            double currentHour = DateTime.Now.Hour;
            string strStartTime = DateTime.Now.ToShortDateString() + " " + Setting.instance.timeStart;
            string strStopTime = DateTime.Now.ToShortDateString() + " " + Setting.instance.timeStop;
            DateTime dtStart = DateTime.Parse(strStartTime);
            DateTime dtStop  = DateTime.Parse(strStopTime);
            //writeStatus(dtStart.ToString());
            //writeStatus(dtStop.ToString());
            if (dtStart < DateTime.Now && DateTime.Now <= dtStop) return true;
            return false;
        }

        private void writeStatus(string status)
        {
            try
            {
                //if (rtLog.Text.Contains(status) && checkNew) return;
                
                if (rtLog.InvokeRequired)
                    rtLog.Invoke(onWriteStatus, status);
                else
                {
                    string logText = ((string.IsNullOrEmpty(rtLog.Text) ? "" : "\r") + string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), status));

                    if (rtLog.Lines.Length > 3000) rtLog.Clear();

                    rtLog.AppendText(logText);
                    rtLog.ScrollToCaret();
                    this.onWriteLog(logText);
                }

            }
            catch (Exception)
            {

            }
        }

        private void LogToFile(string result)
        {
            try
            {
                string filename = m_path + "\\Log\\" + string.Format("log_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

                if (string.IsNullOrEmpty(filename))
                    return;
                StreamWriter streamWriter = new StreamWriter((Stream)System.IO.File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                if (!string.IsNullOrEmpty(result))
                    streamWriter.WriteLine(result);
                streamWriter.Close();
            }
            catch (System.Exception ex)
            {
            }
        }

        private void RestartLogToFile(string result)
        {
            try
            {
                string filename = m_path + "\\Log\\" + string.Format("restart_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                if (string.IsNullOrEmpty(filename))
                    return;
                StreamWriter streamWriter = new StreamWriter((Stream)System.IO.File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                if (!string.IsNullOrEmpty(result))
                    streamWriter.WriteLine(result);
                streamWriter.Close();
            }
            catch (System.Exception ex)
            {
            }
        }

        private void postTokenToServers(string payload)
        {
            CustomEndpoint.sendNewUserTerm("http://89.40.6.53:6002", payload);
            CustomEndpoint.sendNewUserTerm("http://91.121.70.201:9002", payload);
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGeneralSetting frmSet = new frmGeneralSetting();
            frmSet.ShowDialog();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Setting.instance.saveSettingInfo();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        private void clearCookiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        async private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (m_refreshThread != null)
                    m_refreshThread.Abort();
                //FakeUserAction.Intance.stopHumanThread();
                Environment.Exit(-1);
            }
            catch
            {
            }
        }
        private void LoadSetting()
        {
            try
            {
                string strContent = CustomEndpoint.getRequest(string.Format(Setting.instance.serverAddr + "/interface/bot-config/{0}", Setting.instance.betUsername));
                Setting.instance.BotSetting = JsonConvert.DeserializeObject<dynamic>(strContent);

            }
            catch (Exception ex)
            {
                writeStatus(ex.ToString());
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadSetting();
            if (Setting.instance.BotSetting != null)
                writeStatus(Setting.instance.BotSetting.ToString());
            Task.Run(() =>
            {
                writeStatus(string.Format("X:{0} Y:{1}", this.Location.X, this.Location.Y));
            });
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            frmGeneralSetting frm = new frmGeneralSetting();
            DialogResult res = frm.ShowDialog();
            txtUser.Text = Setting.instance.betUsername;
            txtIP.Text = Setting.instance.serverAddr;
        }

        //
        private void procUpdateNetworkStatus(bool isOnline)
        {
            try
            {
                if (isOnline)
                {
                    this.Invoke(new Action(() =>
                    {
                        btnServerStatus.FillColor = Color.LimeGreen;
                    }));

                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        btnServerStatus.FillColor = Color.Red;
                    }));
                }
            }
            catch (Exception e)
            {
            }
        }

        private void procNewTip(List<BetItem> betitemList)
        {
            if (Setting.instance.isWorkingAtDayTime && !CheckIfWorkingFrame())
                return;
            lock (_betList)
            {
                foreach(BetItem newItem in betitemList)
                {
                    bool isNew = true;
                    foreach(BetItem oldItem in _betList)
                    {
                        if(newItem.runnerId == oldItem.runnerId)
                        {
                            isNew = false;
                        }
                    }
                    if (isNew)
                    {
                        _betList.Add(newItem);
                    }
                }
            }
        }

        // Bet Functions
        private bool canStart()
        {
            if (GlobalConstants.state == State.Running)
            {
                writeStatus("The bot has been started already!");
                return false;
            }
            if (string.IsNullOrEmpty(Setting.instance.betUsername) ||
                string.IsNullOrEmpty(Setting.instance.betPassword))
            {
                writeStatus("Pleas enter bet365 account.");
                return false;
            }

            return true;
        }

        private void refreshControls(bool state)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    btnStart.Enabled = state;
                    btnStop.Enabled = !state;
                }));
                GlobalConstants.state = state ? State.Stop : State.Running;
            }
            catch (Exception e)
            {
            }
        }

        async private void toolStripClearCookie_Click(object sender, EventArgs e)
        {
            Task.Run(async () => 
            {
                Win32.SetForegroundWindow(Setting.instance.WindowHandle);
                BetItem betitem = new BetItem();
                betitem.runnerId = "1296789998";
                BetUIController.Intance.DoAddBetUI(betitem);
            });
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            Setting.instance.WriteRegistry("bAutoStart", "0");
            if (!canStart())
                return;
            refreshControls(false);

            Setting.instance.bet365Domain = "bet365.com";
            if (Setting.instance.countryCode.Equals("es"))
            {
                Setting.instance.bet365Domain = "bet365.es";
            }
            else if (Setting.instance.countryCode.Equals("gr"))
            {
                Setting.instance.bet365Domain = "bet365.gr";
            }
            else if (Setting.instance.countryCode.Equals("de"))
            {
                Setting.instance.bet365Domain = "bet365.de";
            }
            else if (Setting.instance.countryCode.Equals("it"))
            {
                Setting.instance.bet365Domain = "bet365.it";
            }
            else if (Setting.instance.countryCode.Contains("au"))
            {
                Setting.instance.bet365Domain = "bet365.com.au";
            }

            Process[] processes  = null;
            if (Setting.instance.isEnabledChrome)
            {
                processes = Process.GetProcessesByName("chrome");
                if (Setting.instance.heightDiff == 1) Setting.instance.heightDiff = 71;
            }
            else if (Setting.instance.isEnabledFirefox) 
            {
                processes = Process.GetProcessesByName("firefox");
                if (Setting.instance.heightDiff == 1) Setting.instance.heightDiff = 85;
            }
            else if (Setting.instance.isEnabledEdge)
            {
                processes = Process.GetProcessesByName("msedge");
                if (Setting.instance.heightDiff == 1) Setting.instance.heightDiff = 71;
            }
            else
            {
                onWriteStatus("Check browser type of setting page and restart app.");
                return;
            }

            foreach (Process p in processes)
            {
                IntPtr windowHandle = p.MainWindowHandle;
                if (windowHandle != IntPtr.Zero) 
                {
                    Win32.SetForegroundWindow(windowHandle);
                    onWriteStatus(string.Format("windowHandle => {0}", windowHandle));
                    Setting.instance.WindowHandle = windowHandle;
                }
            }

            FakeUserAction.CreateInstance(onWriteStatus);
            BetUIController.CreateInstance(onWriteStatus, onWriteLog, m_socketConnector);

            WebsocketServer.CreateInstance(onWriteStatus);
            WebsocketServer.Instance.Start();

            if (m_socketConnector == null)
            {
                m_socketConnector = new SocketConnector(onWriteLog, onWriteStatus, onProcNewTipEvent);
                m_socketConnector.m_handlerProcUpdateNetworkStatus = this.onProcUpdateNetworkStatusEvent;
            }

            m_socketConnector.startListening();
            m_refreshThread = new Thread(refreshThreadFunc);
            m_refreshThread.Start();

            writeStatus("The bot has been started!");
            try
            {
                Setting.instance.innerWidth = int.Parse(BetUIController.Intance.ExecuteScript("document.body.clientWidth", true));
                Setting.instance.innerHeight = int.Parse(BetUIController.Intance.ExecuteScript("document.body.clientHeight", true));
            }
            catch
            {
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                writeStatus("The bot has been stopped!");
                //IOManager.removeBetData();
                refreshControls(true);
                try
                {
                    if (m_refreshThread != null) m_refreshThread.Abort();
                }
                catch
                {
                }
                m_socketConnector.CloseSocket();
                m_socketConnector = null;
                //FakeUserAction.Intance.stopHumanThread();
            }
            catch
            {
            }
        }

        private void tsExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(-1);
        }
        async private void tsRestart_Click(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(1000 * 5);
            }
            catch
            {

            }
            Setting.instance.WriteRegistry("bAutoStart", "1");
            Process.Start(Application.ExecutablePath);
            Process.GetCurrentProcess().Kill();
        }
        private void ExitProcess(string procName)
        {
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    return;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName == procName && proc.Id != Process.GetCurrentProcess().Id)
                        proc.Kill();
                }
            }
            catch (Exception)
            {

            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            writeStatus("refreshBalance");
            Task.Run(async () =>
            {

            });
        }

        private void tsFingerPrint_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {

                    string checkSlipCode = "flashvars.LOGGED_IN";
                    //bool isAdded = ExecuteScript("BetSlipLocator.betSlipManager.betslip.getBetCount()", true) != "0";
                    string jsResult = BetUIController.Intance.ExecuteScript(checkSlipCode, true);
                    writeStatus(checkSlipCode + " = " + jsResult);
                    jsResult = BetUIController.Intance.ExecuteScript("1+1", true);
                    writeStatus("1+1 = " + jsResult);
                }
                catch 
                { 
                }
            });

        }
        private void setValues()
        {
            // General Setting
            Setting.instance.countryCode = txtCountry.Text;
            Setting.instance.serverAddr = txtServerURL.Text;
            Setting.instance.betUsername = txtUsername.Text;
            Setting.instance.betPassword = txtPassword.Text;
            Setting.instance.qrAPI = txtQrAPI.Text;
            Setting.instance.proxyServer = txtProxyUrl.Text;
            Setting.instance.license = txtLicense.Text;

            Setting.instance.isKeepSessionAlive = chkKeepSession.Checked;
            //Setting.instance.isMakeFakeBet = chkFakeBet.Checked;
            //Setting.instance.isComplex = chkComplex.Checked;
            Setting.instance.isDouble = chkDouble.Checked;
            Setting.instance.isWorkingAtDayTime = chkWorkingAtDayTime.Checked;

            Setting.instance.numStake = (double)numStake.Value;
            Setting.instance.anydesk = txtAnydesk.Text;
            Setting.instance.owner = txtOwner.Text;
            // BookieBashing
            //Setting.instance.numFlatStake = (double)numDelayAfterLogin.Value;
            //Setting.instance.numBetCount = (double)numDelayBetweenStartandLoad.Value;
            //Setting.instance.numOddsMin = (double)numDelayLoadandLogin.Value;
            //Setting.instance.isRecordResult = chkRecordResult.Checked;

            //Setting.instance.isEnabledChrome = chkChrome.Checked;
            //Setting.instance.isEnabledFirefox = chkFirefox.Checked;
            //Setting.instance.isEnabledEdge = chkEdge.Checked;

            //Setting.instance.chromePath = txtChromePath.Text;
            //Setting.instance.firefoxPath = txtFirefoxPath.Text;
            //Setting.instance.edgePath = txtEdgePath.Text;


            Setting.instance.heightDiff = (double)numHeightDiff.Value;

            // Delay
            //Setting.instance.isUseUILogin = chkUseUILogin.Checked;

            //Setting.instance.delayStart_Load = (double)numDelayBetweenStartandLoad.Value;
            //Setting.instance.delayLoad_Login = (double)numDelayLoadandLogin.Value;
            //Setting.instance.delayAfterLogin = (double)numDelayAfterLogin.Value;
            //Setting.instance.delayAfterRefresh = (double)numDelayAfterRefresh.Value;
            //Setting.instance.delayBetweenRetries = (double)numDelayBetweenRetries.Value;
            //Setting.instance.delayBetweenBets = (double)numDelayBetweenBets.Value;

            Setting.instance.placingMode = (PLACING_MODE)dmChooseMode.SelectedIndex;

            Setting.instance.timeStart = timeStart.Text;
            Setting.instance.timeStop = timeStop.Text;

            Setting.instance.dolphinUrl = dolphinUrl.Text;
            Setting.instance.dolphinToken = token.Text;
            Setting.instance.dolphinProfileId = profileId.Text;
        }

        private void initValues()
        {
            // General Setting

            txtCountry.Text = Setting.instance.countryCode;
            txtServerURL.Text = Setting.instance.serverAddr;
            txtUsername.Text = Setting.instance.betUsername;
            txtPassword.Text = Setting.instance.betPassword;
            txtLicense.Text = Setting.instance.license;
            txtQrAPI.Text = Setting.instance.qrAPI;
            txtProxyUrl.Text = Setting.instance.proxyServer;

            chkKeepSession.Checked = Setting.instance.isKeepSessionAlive;
            //chkFakeBet.Checked = Setting.instance.isMakeFakeBet;
            //chkComplex.Checked = Setting.instance.isComplex;
            chkWorkingAtDayTime.Checked = Setting.instance.isWorkingAtDayTime;
            numStake.Value = (decimal)Setting.instance.numStake;

            // BookieBashing
            //numDelayAfterLogin.Value = (decimal)Setting.instance.numFlatStake;
            //numDelayBetweenStartandLoad.Value = (decimal)Setting.instance.numBetCount;
            //numDelayLoadandLogin.Value = (decimal)Setting.instance.numOddsMin;

            //chkRecordResult.Checked = Setting.instance.isRecordResult;
            //chkUseUILogin.Checked = Setting.instance.isUseUILogin;

            //chkDouble.Checked = Setting.instance.isDouble;
            //chkChrome.Checked = Setting.instance.isEnabledChrome;
            //chkFirefox.Checked = Setting.instance.isEnabledFirefox;
            //chkEdge.Checked = Setting.instance.isEnabledEdge;
            //txtChromePath.Text = Setting.instance.chromePath;
            //txtFirefoxPath.Text = Setting.instance.firefoxPath;
            //txtEdgePath.Text = Setting.instance.edgePath;

            numHeightDiff.Value = (decimal)Setting.instance.heightDiff;
            txtAnydesk.Text = Setting.instance.anydesk;
            txtOwner.Text = Setting.instance.owner;

            //numDelayBetweenStartandLoad.Value = (decimal)Setting.instance.delayStart_Load;
            //numDelayLoadandLogin.Value = (decimal)Setting.instance.delayLoad_Login;
            //numDelayAfterLogin.Value = (decimal)Setting.instance.delayAfterLogin;
            //numDelayAfterRefresh.Value = (decimal)Setting.instance.delayAfterRefresh;
            //numDelayBetweenRetries.Value = (decimal)Setting.instance.delayBetweenRetries;
            //numDelayBetweenBets.Value = (decimal)Setting.instance.delayBetweenBets;

            dmChooseMode.SelectedIndex = (int)Setting.instance.placingMode;

            timeStart.Text = Setting.instance.timeStart;
            timeStop.Text = Setting.instance.timeStop;

            dolphinUrl.Text = Setting.instance.dolphinUrl;
            token.Text = Setting.instance.dolphinToken;
            profileId.Text = Setting.instance.dolphinProfileId;
        }

        private bool canSet()
        {
            if (string.IsNullOrEmpty(txtCountry.Text))
            {
                txtCountry.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtServerURL.Text))
            {
                txtCountry.Focus();
                return false;
            }

            return true;
        }

        private void btnSaveSoccerbet_Click(object sender, EventArgs e)
        {
            if (!canSet())
                return;

            setValues();
            Setting.instance.saveSettingInfo();
        }

        private void siticoneShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
            initValues();
        }

        private void rtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }
    }
}
