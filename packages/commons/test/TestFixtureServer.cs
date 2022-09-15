using System.Diagnostics;

namespace Deque.AxeCore.Commons.Test.Util
{
    public class TestFixtureServer
    {
        private static Process serverProc;
        private TestFixtureServer() { }
        public static void Start(string pathToCommonsTest)
        {
            string cmdName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                cmdName = "cmd";
            } else {
                cmdName = "sh";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = pathToCommonsTest,
                UseShellExecute = false,
                RedirectStandardInput = true,
                FileName = cmdName
            };
            serverProc = Process.Start(startInfo);

            serverProc.StandardInput.WriteLine("npm run serveFixtures & exit");
        }
        public static void Stop()
        {
            serverProc.Close();
        }
    }
}

