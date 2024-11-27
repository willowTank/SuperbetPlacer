using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools
{
    public class ChromeBrowserSettings
    {
        public int Port { get; set; }
        public bool UseRandomPort { get; set; }
        public string DataDir { get; set; }
        public string[] Args { get; set; }

        public ChromeBrowserSettings()
        {
            Args = new string[]
            {
                "--no-first-run","--disable-default-apps","--no-default-browser-check","--disable-breakpad",
                "--disable-crash-reporter","--no-crash-upload","--deny-permission-prompts",
                "--autoplay-policy=no-user-gesture-required","--disable-prompt-on-repost",
                "--disable-search-geolocation-disclosure","--password-store=basic","--use-mock-keychain",
                "--force-color-profile=srgb","--disable-blink-features=AutomationControlled","--disable-infobars",
                "--disable-session-crashed-bubble","--disable-renderer-backgrounding",
                "--disable-backgrounding-occluded-windows","--disable-background-timer-throttling",
                "--disable-ipc-flooding-protection","--disable-hang-monitor","--disable-background-networking",
                "--metrics-recording-only","--disable-sync","--disable-client-side-phishing-detection",
                "--disable-component-update","--disable-features=TranslateUI,enable-webrtc-hide-local-ips-with-mdns,OptimizationGuideModelDownloading,OptimizationHintsFetching",
                /*"--disable-web-security",*/"--start-maximized"
            };
        }
        public int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
