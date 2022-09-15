using System;
using System.IO;
using System.Reflection;
using System.Text;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Commons.Test.Util;
using Deque.AxeCore.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    [TestFixture]
    [NonParallelizable]
    [Category("RunPartial")]
    public abstract class TestBase : Deque.AxeCore.Selenium.Test.IntegrationTestBase
    {
        protected string axeSource { get; set; } = null;
        protected string axeCrashScript { get; set; } = null;
        protected string dylangConfigPath { get; set; } = null;
        protected string axeRunPartialThrows { get; set; } =
            ";axe.runPartial = () => { throw new Error(\"No runPartial\")}";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var assembly = typeof(TestBase).GetTypeInfo().Assembly;
            axeSource = new BundledAxeScriptProvider().GetScript();

            axeCrashScript = File.ReadAllText(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "fixtures/axe-crasher.js")
            );

            dylangConfigPath = FixturePath("dylang-config.json");

            string runningPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var commonsPath = Path.GetFullPath(Path.Combine(runningPath, @"../../../../..", "commons", "test"));
            TestFixtureServer.Start(commonsPath);
        }

        [OneTimeTearDown]
        public void TearDown()
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

        public static IWebDriver NewDriver()
        {
            EnsureWebdriverPathInitialized(ref ChromeDriverPath, "CHROMEWEBDRIVER", "chromedriver", new ChromeConfig());
            ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(ChromeDriverPath));
            chromeService.SuppressInitialDiagnosticInformation = true;
            var options = new ChromeOptions();
            if (Environment.GetEnvironmentVariable("CI") != null)
            {
                options.AddArgument("--no-sandbox");
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
            }
            return new ChromeDriver(chromeService, options);
        }

        public static string ResourcePath(string filename)
        {
            // https://stackoverflow.com/questions/27181774/get-resources-folder-path-c-sharp
            string runningPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var p = Path.GetFullPath(Path.Combine(runningPath, @"../../../", "RunPartial", filename));
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
            // https://stackoverflow.com/questions/27181774/get-resources-folder-path-c-sharp
            string runningPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var p = Path.GetFullPath(Path.Combine(runningPath, @"../../../../../..", "node_modules", "axe-test-fixtures", "fixtures", filename));
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
