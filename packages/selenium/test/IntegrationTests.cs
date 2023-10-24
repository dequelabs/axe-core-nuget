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

namespace Deque.AxeCore.Selenium.Test
{
    public class IntegrationTests : IntegrationTestBase
    {
        private static readonly string IntegrationTestTargetComplexTargetsFile = Path.Combine(TestFileRoot, @"integration-test-target-complex.html");
        private static readonly string IntegrationTestJsonResultFile = Path.GetFullPath(Path.Combine(TestFileRoot, @"SampleResults.json"));

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
            results.Violations.First().Nodes.First().XPath.Should().NotBeNull();

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

            colorContrast
                .Nodes
                .Select(node => node.Target.ToString())
                .Should().ContainInOrder("p", "[\"iframe\", \"p\"]", "[\"iframe\", [\"#container\", \"p\"]]");
        }

        [Test]
        [TestCase("Chrome")]
        public void ScanRespectsIframeFalseOption(string browser)
        {
            var filename = new Uri("file://" + Path.GetFullPath(IntegrationTestTargetComplexTargetsFile)).AbsoluteUri;
            InitDriver(browser);
            WebDriver.Navigate().GoToUrl(filename);

            var axeResult = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions
                {
                    Iframes = false
                })
                .Analyze();

            var colorContrast = axeResult
                .Violations
                .FirstOrDefault(x => x.Id == "color-contrast");

            colorContrast.Should().NotBeNull();
            colorContrast.Nodes.Should().HaveCount(1); // missing the 2 from the iframe
        }
    }
}
