using System.IO;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Commons.Test.Util;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    [TestFixture]
    [Category("RunPartial")]
    public abstract class TestBase : Deque.AxeCore.Selenium.Test.IntegrationTestBase
    {
        protected string axeSource { get; set; } = null;
        private string _axeCrashScript = null;
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
        private string _axeLargePartial = null;
        protected string axeLargePartial
        {
            get
            {
                if (_axeLargePartial == null)
                {
                    _axeLargePartial = File.ReadAllText(
                        Path.Combine(TestFileRoot, "fixtures", "axe-large-partial.js")
                    );
                }
                return _axeLargePartial;
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
        protected string dylangConfigPath { get; set; } = null;
        protected string axeRunPartialThrows { get; set; } =
            ";axe.runPartial = () => { throw new Error(\"No runPartial\")}";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            axeSource = new BundledAxeScriptProvider().GetScript();

            dylangConfigPath = FixturePath("dylang-config.json");

            var commonsPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(TestFileRoot, @"../../../../..", "commons", "test"));
            TestFixtureServer.Start(commonsPath);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestFixtureServer.Stop();
        }

        protected AxeBuilderOptions CustomSource(string axeSource)
        {
            return new AxeBuilderOptions
            {
                ScriptProvider = new StringAxeScriptProvider(axeSource)
            };
        }
        protected void GoToResource(string resourceFilename)
        {
            WebDriver.Navigate().GoToUrl(ResourceUrl(resourceFilename));
        }

        protected void GoToFixture(string resourceFilename)
        {
            WebDriver.Navigate().GoToUrl(FixtureUrl(resourceFilename));
        }

        protected void GoToUrl(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
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
            var filePath = FixturePath(filename);
            var u = $"file://{filePath}";
            return u;
        }

    }
}
