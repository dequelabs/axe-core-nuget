using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Commons.Test.Util;
using NUnit.Framework;
using Microsoft.Playwright.NUnit;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{
    [TestFixture]
    [Category("RunPartial")]
    public abstract class TestBase : PageTest
    {

        protected static readonly string TestFileRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        protected string axeSource { get; set; } = null;
        private string _axeCrashScript = null;
        private readonly TestServer testServer;
        protected string axeCrashScript
        {
            get
            {
                if (_axeCrashScript == null)
                {
                    _axeCrashScript = File.ReadAllText(
                        Path.Combine(TestFileRoot, "fixtures", "axe-crasher.js")
                    );
                }
                return _axeCrashScript;
            }
        }
        private string _axeForceLegacy = null;
        protected string axeForceLegacy
        {
            get
            {
                if (_axeForceLegacy == null)
                {
                    _axeForceLegacy = File.ReadAllText(
                        Path.Combine(TestFileRoot, "fixtures", "axe-force-legacy.js")
                    );
                }
                return _axeForceLegacy;
            }
        }
        private string _axeCoreLegacy = null;
        protected string axeCoreLegacy
        {
            get
            {
                if (_axeCoreLegacy == null)
                {
                    _axeCoreLegacy = File.ReadAllText(
                        Path.Combine(TestFileRoot, "fixtures", "axe-core@legacy.js")
                    );
                }
                return _axeCoreLegacy;
            }
        }
        protected string axeRunPartialThrows { get; set; } =
            ";axe.runPartial = () => { throw new Error(\"No runPartial\")}";

        public TestBase()
        {
            testServer = new("fixtures");
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            axeSource = new BundledAxeScriptProvider().GetScript();

            var commonsPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(TestFileRoot, @"../../../../..", "commons", "test"));
            await testServer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await testServer.StopAsync();
        }

        protected string CustomSource(string axeSource)
        {
            return axeSource;
        }
        protected Task GoToResource(string resourceFilename)
        {
            return GoToUrl(ResourceUrl(resourceFilename));
        }

        protected Task GoToFixture(string resourceFilename)
        {
            return GoToUrl(FixtureUrl(resourceFilename));
        }

        protected Task GoToUrl(string url)
        {
            return Page!.GotoAsync(url);
        }

        protected Task<AxeResult> RunWithCustomAxe(string customSource)
        {
            return Page!.RunAxe(null, null, customSource);
        }

        protected Task<AxeResult> RunLegacyWithCustomAxe(string customSource)
        {
#pragma warning disable CS0618
            return Page!.RunAxeLegacy(null, null, customSource);
#pragma warning restore CS0618
        }

        public static string ResourcePath(string filename)
        {
            var p = Path.GetFullPath(Path.Combine(TestFileRoot, @"../../..", "RunPartial", filename));
            return p;
        }

        public static string ResourceUrl(string filename)
        {
            var filePath = ResourcePath(filename);
            var u = $"file://{filePath}";
            return u;
        }

        public static string FixturePath(string filename)
        {
            var p = Path.GetFullPath(Path.Combine(TestFileRoot, "fixtures", filename));
            return p;
        }

        public static string FixtureUrl(string filename)
        {
            System.Uri uri = new(TestServer.BaseUri, filename);
            return uri.ToString();
        }

    }
}
