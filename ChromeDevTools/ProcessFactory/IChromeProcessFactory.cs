namespace MasterDevs.ChromeDevTools
{
    public interface IChromeProcessFactory
    {
        IChromeProcess Create(ChromeBrowserSettings chromeBrowserSettings);
        void DisposePreviousProcess(string chromeDir, string cacheDir);
    }
}