using ChromeDevTools.Extensions;
using ChromeDevTools.ProcessFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace MasterDevs.ChromeDevTools
{
    public class ChromeProcessFactory : AbstractProcessFactory
    {
        public IDirectoryCleaner DirectoryCleaner { get; set; }
        public string ChromePath { get; set; }

        private string _chromeDir;

        public ChromeProcessFactory(IDirectoryCleaner directoryCleaner, string chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe")
        {
            DirectoryCleaner = directoryCleaner;
            ChromePath = chromePath;
            _chromeDir = Path.GetDirectoryName(chromePath);
        }

        public override IChromeProcess Create(ChromeBrowserSettings chromeBrowserSettings)
        {
            //string path = Path.GetRandomFileName();
            string path = "user";
            var directoryInfo = Directory.CreateDirectory(Path.Combine(chromeBrowserSettings.DataDir, path));

            var port = chromeBrowserSettings.UseRandomPort ? chromeBrowserSettings.GetFreeTcpPort() : chromeBrowserSettings.Port;
            var explicitlyPort = port + 2;

            var chromeProcessArgs = new List<string>
            {
                $"--remote-debugging-port={port}",
                $"--explicitly-allowed-ports={explicitlyPort}",
                //userDirectoryArg,
                //"--bwsi",
                "--no-first-run",
            };

            if (!string.IsNullOrEmpty(chromeBrowserSettings.DataDir))
            {
                chromeProcessArgs.Add($"--user-data-dir=\"{directoryInfo.FullName}\\Data\\user\"");
                chromeProcessArgs.Add($"--media-cache-dir=\"{directoryInfo.FullName}\\Data\\media\"");
                chromeProcessArgs.Add($"--disk-cache-dir=\"{directoryInfo.FullName}\\Data\\cache\"");

                //chromeProcessArgs.Add($"--disk-cache-dir=\"{chromeBrowserSettings.DataDir}\"");
                //disk-cache-dir
            }
            //chromeProcessArgs.Add($"--disk-cache-dir=\"{"TEST"}\"");
            if (chromeBrowserSettings.Args != null)
            {
                chromeProcessArgs.AddRange(chromeBrowserSettings.Args);
            }
            var processStartInfo = new ProcessStartInfo(ChromePath, string.Join(" ", chromeProcessArgs));
            var chromeProcess = Process.Start(processStartInfo);

            string remoteDebuggingUrl = "http://localhost:" + port;
            //return new LocalChromeProcess(new Uri(remoteDebuggingUrl), () => DirectoryCleaner.Delete(directoryInfo), chromeProcess);
            return new LocalChromeProcess(new Uri(remoteDebuggingUrl), () => { }, chromeProcess.Id);

        }
    }
}