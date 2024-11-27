using FirefoxBet365Placer.Constants;
using FirefoxBet365Placer.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FirefoxBet365Placer.Controller
{
    public class WebsocketClient
    {
        private static WebsocketClient _instance;
        private onWriteStatusEvent m_handlerWriteStatus;
        private onProcNewTipEvent m_handlerProcNewTip;
        private WebSocket wssv;
        public bool isConnected;
        public Thread _pingThread = null;
        public static WebsocketClient Instance { get { return _instance; } }

        public bool _isVerified { get; private set; }

        public WebsocketClient(onWriteStatusEvent handlerWriteStatusEvent, onProcNewTipEvent handlerProcNewTipEvent)
        {
            m_handlerWriteStatus = handlerWriteStatusEvent;
            m_handlerProcNewTip = handlerProcNewTipEvent;
        }

        public static void CreateInstance(onWriteStatusEvent handlerWriteStatusEvent, onProcNewTipEvent handlerProcNewTipEvent)
        {
            _instance = new WebsocketClient(handlerWriteStatusEvent, handlerProcNewTipEvent);
        }

        public void Start()
        {
            return;
            wssv = new WebSocket("wss://client.zenghh.fun/2/");
            wssv.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            wssv.OnMessage += OnMessage;
            wssv.OnOpen  += OnOpen;
            wssv.OnClose += OnClose;
            wssv.OnError += OnError;
            wssv.Connect();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            return;
            isConnected = true;
            _isVerified = false;
            string strAuthPayload = "{\"msg\":\"auth\",\"acc\":\"" + Setting.instance.betUsername + "\",\"url\":\"www.bet365.com\",\"ver\":\"1\"}";
            wssv.Send(strAuthPayload);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            isConnected = false;
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            isConnected = false;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            return;
            try
            {
                JObject jsonMessage = JsonConvert.DeserializeObject<JObject>(e.Data);
                string strMsgCode = jsonMessage.SelectToken("msg").ToString().ToLower();
                if (strMsgCode.Contains("auth") && e.Data.Contains("success"))
                {
                    _isVerified = true;
                    m_handlerWriteStatus("Zhong connection has been verified.");
                    string currencyCode = BetController.Intance.ExecuteScript("Locator.user.currencyCode", true);
                    string strCurrencyData = "{\"msg\":\"currency\",\"currencyCode\":\"BGN\",\"currencyRace\":2.3117}";
                    wssv.Send(strCurrencyData);

                    try
                    {
                        if (_pingThread != null) _pingThread.Abort();
                    }
                    catch
                    {
                    }
                    _pingThread = new Thread(pingThreadFunc);
                    _pingThread.Start();
                }
                else if (strMsgCode.Contains("pong"))
                {
                }
                else if (strMsgCode.Contains("bet"))
                {
                    string home = jsonMessage.SelectToken("en_home").ToString();
                    string away = jsonMessage.SelectToken("en_away").ToString();
                    string sel = jsonMessage.SelectToken("sel").ToString();
                    string uuid = jsonMessage.SelectToken("uuid").ToString();
                    string stake = jsonMessage.SelectToken("stake").ToString();

                    string FI = jsonMessage.SelectToken("pa").SelectToken("FI").ToString();
                    string ID = jsonMessage.SelectToken("pa").SelectToken("ID").ToString();
                    string IT = jsonMessage.SelectToken("pa").SelectToken("IT").ToString();
                    string NA = jsonMessage.SelectToken("pa").SelectToken("NA").ToString();
                    string OD = jsonMessage.SelectToken("pa").SelectToken("OD").ToString();
                    string OR = jsonMessage.SelectToken("pa").SelectToken("OR").ToString();
                    string SU = jsonMessage.SelectToken("pa").SelectToken("SU").ToString();
                    string FOD = jsonMessage.SelectToken("pa").SelectToken("FOD").ToString();
                    string HA = jsonMessage.SelectToken("pa").SelectToken("HA").ToString();

                    string strBS = string.Format("pt=N#o={0}#f={1}#fp={2}#so=0#c={3}#mt=11#id={1}-{2}Y#|TP=BS{1}-{2}#", OD, FI, ID, 1);
                    if (string.IsNullOrEmpty(HA) == false)
                    {
                        string strHandi = HA.Replace("+", "");
                        strBS = string.Format("pt=N#o={0}#f={1}#fp={2}#so=0#c={3}#ln={4}#mt=11#id={1}-{2}Y#|TP=BS{1}-{2}#", OD, FI, ID, 1, strHandi);
                    }

                    BetItem betitem = new BetItem();
                    betitem.sport_id = 1;
                    betitem.match = home + " vs " + away;
                    betitem.bs = strBS;
                    betitem.runnerId = ID;
                    betitem.tipster = "messi";
                    betitem.Leader = "messi";
                    betitem.pick = sel;
                    betitem.odds = Utils.ParseToDouble(FOD);
                    betitem.eventDistance = 0;
                    betitem.oddsDistance = 0;
                    betitem.isDouble = false;
                    betitem.selectionCount = 1;
                    betitem.PD = "/#/IP/B1";
                    betitem.bEW = false;
                    betitem.betId = uuid;
                    betitem.stake = Utils.ParseToDouble(stake);
                    betitem.source = SOURCE.TIPSTER;
                    List<BetItem> betList = new List<BetItem>();
                    betList.Add(betitem);
                    m_handlerProcNewTip(betList);
                }
                else
                {
                    m_handlerWriteStatus(e.Data);
                }
            }
            catch
            {

            }
        }

        public void pingThreadFunc()
        {
            while (isConnected)
            {
                Thread.Sleep(20 * 1000);
                string strPingData = "{\"msg\":\"ping\",\"acc\":\"" + Setting.instance.betUsername + "\"}";
                wssv.Send(strPingData);
            }
        }
        public void Stop()
        {
            wssv.Close();
        }

        public void SendData(string message)
        {
            try
            {
                m_handlerWriteStatus(message);
                //if(isConnected && _isVerified) wssv.Send(message);
            }
            catch
            {
            }
        }
        public void ExecuteScript(string jsCode)
        {
            dynamic payload = new JObject();
            payload.type = "jscode";
            payload.body = jsCode;
            SendData(payload.ToString());
        }
        public void HandleIncomingMessages(string data)
        {
            JObject jsonData = JsonConvert.DeserializeObject<JObject>(data);
            BetUIController.Intance.WebResourceResponseReceived(jsonData);
        }
    }
}
