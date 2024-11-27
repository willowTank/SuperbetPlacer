using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirefoxBet365Placer
{
    static class Program
    {
        static string remoteVersionURL = "/interface/program-ce-version";
        static String m_updateFileUrl = "/CeRelease({0}).zip";
        static String m_path = string.Empty;
        static Dictionary<string, object> configureData;
        static String m_baseUrl = "http://94.23.39.141:5002";

        static String m_version;
        static String m_softname;
        static String m_placerExe = "Bet365";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static void readConfig()
        {
            m_baseUrl = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).GetValue("serverAddr", (object)"1.0").ToString();
            m_version = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).GetValue("ce-version", (object)"1.0").ToString();
            if (string.IsNullOrEmpty(m_version))
            {
                m_version = "1.0";
            }
        }
        static void PerformMainActivity()
        {
            try
            {
                WebClient webClient = new WebClient();
                readConfig();
                // Format:
                //	<version> <url> <hash>
                string downloadUrl = string.Format("{0}{1}?_={2}", m_baseUrl, remoteVersionURL, getTick());
                string remoteVersionText = webClient.DownloadString(m_baseUrl + remoteVersionURL).Trim();
                Version localVersion = new Version(m_version);
                Version remoteVersion = new Version(remoteVersionText);

                if (remoteVersion != localVersion)
                {
                    StartBetUpdater();
                }
            }
            catch (Exception ex)
            {
            }
        }

        static long getTick()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long timestamp = (long)t.TotalMilliseconds;
            return timestamp;
        }

        static public string ReadAccountInfo()
        {
            try
            {
                string fileContent = File.ReadAllText("account.txt");
                string[] tempArr = fileContent.Split(':');
                return tempArr[0];
            }
            catch (Exception)
            {

            }
            return ":";
        }

        public static void StartBetUpdater()
        {
            try
            {
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
    }
}
