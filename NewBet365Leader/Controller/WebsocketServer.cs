using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FirefoxBet365Placer.Controller
{
    public class WebsocketServer
    {
        private static WebsocketServer _instance;
        private onWriteStatusEvent m_handlerWriteStatus;
        private WebSocketServer wssv;

        public static WebsocketServer Instance { get { return _instance;  } }

        public WebsocketServer(onWriteStatusEvent handlerWriteStatusEvent) 
        {
            m_handlerWriteStatus = handlerWriteStatusEvent;
        }

        public static void CreateInstance(onWriteStatusEvent handlerWriteStatusEvent) 
        {
            _instance = new WebsocketServer(handlerWriteStatusEvent);
        }

        public void Start()
        {
            for(int i = 0; i < 10; i++)
            {
                try
                {
                    wssv = new WebSocketServer((int)Setting.instance.numAgentPort);
                    wssv.AddWebSocketService<FirefoxInterface>("/firefox");
                    wssv.Start();
                    m_handlerWriteStatus(string.Format("Websocket server is listening on {0}", (int)Setting.instance.numAgentPort));
                    break;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
            
            
        }

        public void Stop()
        {
            wssv.Stop();
        }

        public void SendData(string message)
        {
            try
            {
                wssv.WebSocketServices.Broadcast(message);
            }
            catch
            {

            }
        }
        public void ExecuteScript(string jsCode) 
        {
            dynamic payload = new JObject();
            payload.type = "jscode";
            payload.hash = jsCode.GetHashCode().ToString();
            payload.body = jsCode;
            SendData(payload.ToString());
        }
        public void HandleIncomingMessages(string data)
        {
            JObject jsonData = JsonConvert.DeserializeObject<JObject>(data);
            BetUIController.Intance.WebResourceResponseReceived(jsonData);
        }
    }

    public class FirefoxInterface : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            WebsocketServer.Instance.HandleIncomingMessages(e.Data);
        }
    }
}
