using ChromeDevTools.Extensions;
using MasterDevs.ChromeDevTools;
using System.Diagnostics;
using System.Linq;

namespace ChromeDevTools.ProcessFactory
{
    public abstract class AbstractProcessFactory : IChromeProcessFactory
    {
        public abstract IChromeProcess Create(ChromeBrowserSettings chromeBrowserSettings);

        public void DisposePreviousProcess(string chromeDir, string cacheDir)
        {
            if (string.IsNullOrEmpty(cacheDir))
            {
                return;
            }

            var processes = Process.GetProcessesByName("chrome").ToArray();

            if (!string.IsNullOrEmpty(chromeDir))
            {
                processes = processes.Where(c =>
                  c?.MainModule != null &&
                  !string.IsNullOrEmpty(c.MainModule.FileName) &&
                  c.MainModule.FileName.Contains(chromeDir)).ToArray();
            }

            foreach (var process in processes)
            {
                var args = process.GetCommandLine();
                if (!string.IsNullOrEmpty(args) && args.Contains(cacheDir))
                {
                    process.Kill();
                }
            }
        }
    }
}
