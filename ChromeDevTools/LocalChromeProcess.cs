using System;
using System.Diagnostics;

namespace MasterDevs.ChromeDevTools
{
    public class LocalChromeProcess : RemoteChromeProcess
    {
        public long processId;
        public LocalChromeProcess(Uri remoteDebuggingUri, Action disposeUserDirectory, long processId)
            : base(remoteDebuggingUri)
        {
            DisposeUserDirectory = disposeUserDirectory;
            this.processId = processId;
        }

        public Action DisposeUserDirectory { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            //try
            //{
            //    Process.Kill();
            //    Process.WaitForExit();
            //    //            Process.Close();
            //    DisposeUserDirectory();
            //}
            //catch (Exception e)
            //{

            //}
        }
    }
}