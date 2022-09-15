using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;
using Deque.AxeCore.Commons;

// Setup parallelization
[assembly: Parallelizable(ParallelScope.All)]

namespace Deque.AxeCore.Selenium.Test
{
    [TestFixture]
    [Category("Integration")]
    public abstract class IntegrationTestBase
    {
        private readonly ConcurrentDictionary<string, IWebDriver> localDriver = new ConcurrentDictionary<string, IWebDriver>();
        private readonly ConcurrentDictionary<string, WebDriverWait> localWaitDriver = new ConcurrentDictionary<string, WebDriverWait>();

        protected static string ChromeDriverPath = null;
        protected static string FirefoxDriverPath = null;

        public IWebDriver WebDriver
        {
            get
            {
                IWebDriver value;
                localDriver.TryGetValue(GetFullyQualifiedTestName(), out value);
                return value;
            }

            set
            {
                localDriver.AddOrUpdate(GetFullyQualifiedTestName(), value, (oldkey, oldvalue) => value);
            }
        }

        public WebDriverWait Wait
        {
            get
            {
                WebDriverWait value;
                localWaitDriver.TryGetValue(GetFullyQualifiedTestName(), out value);
                return value;
            }

            set
            {
                localWaitDriver.AddOrUpdate(GetFullyQualifiedTestName(), value, (oldkey, oldvalue) => value);
            }
        }

        protected static readonly string TestFileRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string IntegrationTestTargetSimpleFile = Path.Combine(TestFileRoot, @"integration-test-simple.html");

        protected const string mainElementSelector = "main";

        [TearDown]
        public virtual void TearDown()
        {
            WebDriver?.Quit();
            WebDriver?.Dispose();
        }

        protected void LoadSimpleTestPage()
        {
            var filename = new Uri("file://" + IntegrationTestTargetSimpleFile).AbsoluteUri;
            WebDriver.Navigate().GoToUrl(filename);

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));
        }

        protected void InitDriver(string browser)
        {
            string headedEnvVar = Environment.GetEnvironmentVariable("HEADED");
            bool headless = headedEnvVar == null || headedEnvVar == "" || headedEnvVar == "0";

            switch (browser.ToUpper())
            {
                case "CHROME":
                    EnsureWebdriverPathInitialized(ref ChromeDriverPath, "CHROMEWEBDRIVER", "chromedriver", new ChromeConfig());

                    ChromeOptions chromeOptions = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    if (headless)
                    {
                        chromeOptions.AddArgument("--headless");
                    }
                    chromeOptions.AddArgument("no-sandbox");
                    chromeOptions.AddArgument("--log-level=3");
                    chromeOptions.AddArgument("--silent");
                    chromeOptions.AddArgument("--allow-file-access-from-files");

                    ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(ChromeDriverPath));
                    chromeService.SuppressInitialDiagnosticInformation = true;
                    WebDriver = new ChromeDriver(chromeService, chromeOptions);

                    break;

                case "FIREFOX":
                    EnsureWebdriverPathInitialized(ref FirefoxDriverPath, "GECKOWEBDRIVER", "geckodriver", new FirefoxConfig());

                    FirefoxOptions firefoxOptions = new FirefoxOptions();
                    if (headless)
                    {
                        firefoxOptions.AddArgument("-headless");
                    }

                    FirefoxDriverService firefoxService = FirefoxDriverService.CreateDefaultService(Path.GetDirectoryName(FirefoxDriverPath));
                    firefoxService.SuppressInitialDiagnosticInformation = true;
                    WebDriver = new FirefoxDriver(firefoxService, firefoxOptions);
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(20));
            WebDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(20);
            WebDriver.Manage().Window.Maximize();
        }

        protected static void EnsureWebdriverPathInitialized(ref string driverPath, string dirEnvVar, string binaryName, IDriverConfig driverManagerConfig)
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

        protected static string GetFullyQualifiedTestName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}

