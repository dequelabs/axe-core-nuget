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
    public abstract class TestBase
    {
        private static string ChromeDriverPath = null;
        protected IWebDriver driver { get; set; }

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

            driver = NewDriver();
            string runningPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var commonsPath = Path.GetFullPath(Path.Combine(runningPath, @"../../../../..", "commons", "test"));
            TestFixtureServer.Start(commonsPath);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Close();
            driver.Quit();
            TestFixtureServer.Stop();
        }

        protected AxeBuilderOptions CustomSource(string axeSource)
        {
            return new AxeBuilderOptions
            {
                ScriptProvider = new StringAxeScriptProvider(axeSource)
            };
        }
        protected void GoToResource(string url)
        {
            driver.Navigate().GoToUrl(ResourceUrl(url));
        }

        protected void GoToFixture(string url)
        {
            driver.Navigate().GoToUrl(FixtureUrl(url));
        }

        protected void GoToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        protected void AssertHasViolations(AxeResult results, params string[] rules)
        {
            var violations = results.Violations;
            foreach (var violation in violations)
            {
                Console.WriteLine($">> {violation.Id}");
            }

            Assert.That(violations.Length, Is.EqualTo(rules.Length));

            foreach (var rule in rules)
            {
                var found = false;
                foreach (var vio in violations)
                {
                    if (vio.Id == rule)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.Error.WriteLine($"Expected '{rule}' violation but could not find.");
                    Assert.IsTrue(found);
                }
            }
        }

        private static void EnsureWebdriverPathInitialized(ref string driverPath, string dirEnvVar, string binaryName, IDriverConfig driverManagerConfig)
        {
            LazyInitializer.EnsureInitialized(ref driverPath, () =>
            {
                var dirFromEnv = Environment.GetEnvironmentVariable(dirEnvVar);
                if (dirFromEnv != null)
                {
                    return $"{dirFromEnv}/${binaryName}";
                }
                else
                {
                    return new DriverManager().SetUpDriver(driverManagerConfig);
                }
            });
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
