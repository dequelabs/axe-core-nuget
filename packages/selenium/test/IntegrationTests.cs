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
    public class IntegrationTests
    {
        private readonly ConcurrentDictionary<string, IWebDriver> localDriver = new ConcurrentDictionary<string, IWebDriver>();
        private readonly ConcurrentDictionary<string, WebDriverWait> localWaitDriver = new ConcurrentDictionary<string, WebDriverWait>();

        private static string ChromeDriverPath = null;
        private static string FirefoxDriverPath = null;

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

        private static readonly string TestFileRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string IntegrationTestTargetSimpleFile = Path.Combine(TestFileRoot, @"integration-test-simple.html");
        private static readonly string IntegrationTestTargetComplexTargetsFile = Path.Combine(TestFileRoot, @"integration-test-target-complex.html");
        private static readonly string IntegrationTestJsonResultFile = Path.GetFullPath(Path.Combine(TestFileRoot, @"SampleResults.json"));

        private const string mainElementSelector = "main";

        [TearDown]
        public virtual void TearDown()
        {
            WebDriver?.Quit();
            WebDriver?.Dispose();
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void FullPageScanFindsExpectedViolations(string browser)
        {
            InitDriver(browser);
            LoadSimpleTestPage();

            var outputFile = $@"{TestContext.CurrentContext.WorkDirectory}/raw-axe-results.json";
            var timeBeforeScan = DateTime.Now;

            var builder = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions() { XPath = true })
                .WithTags("wcag2a", "wcag2aa")
                .DisableRules("color-contrast")
                .WithOutputFile(outputFile);

            var results = builder.Analyze();
            results.Violations.FirstOrDefault(v => v.Id == "color-contrast").Should().BeNull();
            results.Violations.FirstOrDefault(v => !v.Tags.Contains("wcag2a") && !v.Tags.Contains("wcag2aa")).Should().BeNull();
            results.Violations.Should().HaveCount(2);
            results.Violations.First().Nodes.First().XPath.Should().NotBeNullOrEmpty();

            File.GetLastWriteTime(outputFile).Should().BeOnOrAfter(timeBeforeScan);
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void SingleElementScanFindsExpectedViolations(string browser)
        {
            InitDriver(browser);
            LoadSimpleTestPage();

            var mainElement = Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));

            AxeResult results = WebDriver.Analyze(mainElement);
            results.Violations.Should().HaveCount(3);
        }

        [Test]
        [TestCase("Chrome")]
        public void ScanFindsViolationsInShadowDomIframes(string browser)
        {
            var filename = new Uri("file://" + Path.GetFullPath(IntegrationTestTargetComplexTargetsFile)).AbsoluteUri;
            InitDriver(browser);
            WebDriver.Navigate().GoToUrl(filename);
            var axeResult = new AxeBuilder(WebDriver).Analyze();

            var colorContrast = axeResult
                .Violations
                .FirstOrDefault(x => x.Id == "color-contrast");

            colorContrast.Should().NotBeNull();
            colorContrast.Nodes.Should().HaveCount(3); // including 1 from the top-level frame and 2 from the iframe

            var shadowDomIframeTargetNode = colorContrast
                .Nodes
                .Where(x => x.Target.Any(node => node.Selectors.Any()))
                .Select(x => x.Target.Last())
                .First();

            shadowDomIframeTargetNode.Should().NotBeNull();
            shadowDomIframeTargetNode.Selectors.Should().HaveCount(2);
        }

        [Test]
        [TestCase("Chrome")]
        public void ScanRespectsIframeFalseOption(string browser)
        {
            var filename = new Uri("file://" + Path.GetFullPath(IntegrationTestTargetComplexTargetsFile)).AbsoluteUri;
            InitDriver(browser);
            WebDriver.Navigate().GoToUrl(filename);

            var axeResult = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions {
                    Iframes = false
                })
                .Analyze();

            var colorContrast = axeResult
                .Violations
                .FirstOrDefault(x => x.Id == "color-contrast");

            colorContrast.Should().NotBeNull();
            colorContrast.Nodes.Should().HaveCount(1); // missing the 2 from the iframe
        }

        private void LoadSimpleTestPage()
        {
            var filename = new Uri("file://" + IntegrationTestTargetSimpleFile).AbsoluteUri;
            WebDriver.Navigate().GoToUrl(filename);

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));
        }

        private void InitDriver(string browser)
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
                    if (headless) {
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
                    if (headless) {
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

        private static void EnsureWebdriverPathInitialized(ref string driverPath, string dirEnvVar, string binaryName, IDriverConfig driverManagerConfig) {
            LazyInitializer.EnsureInitialized(ref driverPath, () => {
                var dirFromEnv = Environment.GetEnvironmentVariable(dirEnvVar);
                if (dirFromEnv != null) {
                    return $"{dirFromEnv}/${binaryName}";
                } else {
                    return new DriverManager().SetUpDriver(driverManagerConfig);
                }
            });
        }

        private static string GetFullyQualifiedTestName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}
