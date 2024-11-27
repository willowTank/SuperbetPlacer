using System.Threading;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools
{
    public static class ChromeSessionExtensions
    {
        public static Task<CommandResponse<T>> SendAsync<T>(this IChromeSession session, ICommand<T> parameter)
        {
            if (session == null)
                return null;

            try
            {
                return session.SendAsync(parameter, CancellationToken.None);
            }
            catch
            {
                return null;
            }
        }

        public static Task<ICommandResponse> SendAsync<T>(this IChromeSession session)
        {
            if (session == null)
                return null;

            try
            {
                return session.SendAsync<T>(CancellationToken.None);
            }
            catch
            {
                return null;
            }
        }
    }
}
