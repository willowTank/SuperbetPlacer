using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Web.Script.Serialization;
using System.Threading;

namespace BetUpdater
{
    class Program
    {
        static string remoteVersionURL = "/interface/program-pw-version";
        static String m_updateFileUrl = "/PwRelease({0}).zip";
        static String m_path = string.Empty;
        static Dictionary<string, object> configureData;
        static String m_baseUrl = "http://185.56.169.162:5002";
        
        static String m_version;
        static String m_softname;
        static String m_placerExe = "Bet365";

        static void readConfig()
        {
            m_baseUrl = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).GetValue("serverAddr", (object)"1.0").ToString();
            m_version = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).GetValue("pw-version", (object)"1.0").ToString();
            if (string.IsNullOrEmpty(m_version))
            {
                m_version = "1.0";
            }
        }

        static void Main(string[] args)
        {
            createLogFolders();
            try
            {
                //m_baseUrl = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Thiago-B365").GetValue("serverAddr", (object)"http://45.153.241.205:7002").ToString();
                PerformMainActivity();
            }
            catch(Exception ex)
            {
                LogToFile("[Launcher] Exception in Main: " + ex.ToString());
            }
        }

        static void PerformMainActivity()
        {
            try
            {
                WebClient webClient = new WebClient();
                readConfig();
                LogToFile("Checking for updates...");
                // Format:
                //	<version> <url> <hash>
                string downloadUrl = string.Format("{0}{1}?_={2}", m_baseUrl, remoteVersionURL, getTick());
                string remoteVersionText = webClient.DownloadString(m_baseUrl + remoteVersionURL).Trim();
                LogToFile(string.Format("m_version = {0}", m_version));
                LogToFile(string.Format("remoteVersionText = {0}", remoteVersionText));
                Version localVersion = new Version(m_version);
                Version remoteVersion = new Version(remoteVersionText);

                if (remoteVersion != localVersion)
                {
                    //while (!ExitProcess()) ;
                    // There is a new version on the server!
                    LogToFile("There is a new version available on the server.");
                    LogToFile(string.Format("Current Version: {0}, New version: {1}", localVersion, remoteVersion));
                    string contentUrl = string.Format("{0}{1}?_={2}", m_baseUrl, string.Format(m_updateFileUrl, remoteVersion), getTick());
                    PerformUpdate(contentUrl);
                    Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).SetValue("pw-version", remoteVersion);
                    string ExitForUpdate = Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("Bet365-" + ReadAccountInfo()).GetValue("ExitForUpdate", (object)"0").ToString();
                    StartBetPlacer();
                }
                else
                {
                    StartBetPlacer();
                }
            }
            catch (Exception ex)
            {
                LogToFile("[Launcher] Exception in PerformMainActivity: " + ex.ToString());
            }
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
        static void StartBetPlacer()
        {
            LogToFile("BetPlacer Started!");
            ProcessStartInfo startInfo = new ProcessStartInfo(m_placerExe+ ".exe");
            Process checkerProcess = new Process();
            checkerProcess.StartInfo = startInfo;
            checkerProcess.EnableRaisingEvents = true;
            checkerProcess.Start();
        }

        static long getTick()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long timestamp = (long)t.TotalMilliseconds;
            return timestamp;
        }

        static bool PerformUpdate(string remoteUrl)
        {
            try
            {
                LogToFile("Beginning update.");
                string downloadDestination = Path.GetTempFileName();

                LogToFile(string.Format("Downloading {0} to {1} - ", remoteUrl, downloadDestination));
                WebClient downloadifier = new WebClient();
                downloadifier.DownloadFile(remoteUrl, downloadDestination);
                LogToFile("done.");

                LogToFile("Validating download - ");
                LogToFile("ok.");

                // Since the download doesn't appear to be bad at first sight, let's extract it
                Console.Write("Extracting archive - ");
                string extractTarget = @"./downloadedFiles";
                ZipFile.ExtractToDirectory(downloadDestination, extractTarget);
                // Copy the extracted files and replace everything in the current directory to finish the update
                // C# doesn't easily let us extract & replace at the same time
                foreach (string newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
                    }
                    catch (Exception)
                    {
                    }
                }
                LogToFile("done.");

                // Clean up the temporary files
                LogToFile("Cleaning up - ");
                Directory.Delete(extractTarget, true);
                LogToFile("done.");
            }
            catch
            {

            }
            

            return true;
        }

        /// <summary>
        /// Gets the SHA1 hash from file.
        /// Adapted from https://stackoverflow.com/a/16318156/1460422
        /// </summary>
        /// <param name="fileName">The filename to hash.</param>
        /// <returns>The SHA1 hash from file.</returns>
        static string GetSHA1HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] byteHash = sha1.ComputeHash(file);
            file.Close();

            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
                hashString.Append(byteHash[i].ToString("x2"));
            return hashString.ToString();
        }

        static private void createLogFolders()
        {
            m_path = Directory.GetCurrentDirectory();
            if (!Directory.Exists(m_path + "\\Log\\"))
                Directory.CreateDirectory(m_path + "\\Log\\");
        }

        static private void LogToFile(string result)
        {
            try
            {
                string LogTime = DateTime.Now.ToString("HH:mm:ss");
                string logText = LogTime + "=> " + result;
                string filename = m_path + "\\Log\\" + string.Format("log_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine(logText);
                if (string.IsNullOrEmpty(filename))
                    return;
                StreamWriter streamWriter = new StreamWriter((Stream)System.IO.File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                if (!string.IsNullOrEmpty(result))
                    streamWriter.WriteLine(logText);
                streamWriter.Close();

            }
            catch (System.Exception ex)
            {
            }
        }

        static private bool ExitProcess()
        {
            bool bRet = true;
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    bRet = false;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName.Contains(m_placerExe))
                        proc.Kill();
                }

                liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    bRet = false;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName.Contains(m_placerExe))
                        bRet = false;
                }

            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }


        static private bool CheckPlacerRunning()
        {
            bool bRet = false;
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    bRet = false;

                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName.Contains(m_placerExe))
                    {
                        return true;
                    }
                }

            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }

        static private bool CheckPrevInstance()
        {
            bool bRet = false;
            try
            {
                Process[] liveProcess = Process.GetProcesses();
                if (liveProcess == null)
                    bRet = false;
                int count = 0;
                foreach (Process proc in liveProcess)
                {
                    if (proc.ProcessName == "Bet365")
                        count++;
                }
                if (count > 1)
                {
                    return true;
                }

            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }
    }
}
