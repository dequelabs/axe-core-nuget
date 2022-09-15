using System.Diagnostics;

namespace Deque.AxeCore.Commons.Test.Util
{
    public class TestFixtureServer
    {
        private static Process serverProc;
        private TestFixtureServer() { }
        public static void Start(string pathToCommonsTest)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = pathToCommonsTest,
                RedirectStandardInput = true,
                FileName = "cmd"
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

