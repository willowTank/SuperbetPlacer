using ChromeDevTools.ProcessFactory;
using System;

namespace MasterDevs.ChromeDevTools
{
    public class CefFactory : AbstractProcessFactory
    {
        public override IChromeProcess Create(ChromeBrowserSettings chromeBrowserSettings)
        {
            return new CefChrome(new Uri("http://localhost:" + (chromeBrowserSettings.UseRandomPort ? chromeBrowserSettings.GetFreeTcpPort() : chromeBrowserSettings.Port)));
        }
    }
}
