using System;
using OpenQA.Selenium;
using System.Linq;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class UseLegacyTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldRunLegacyAxe(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

#pragma warning disable CS0618
            var results = new AxeBuilder(WebDriver, CustomSource(axeCoreLegacy))
                .UseLegacyMode(true)
                .Analyze();
#pragma warning restore CS0618

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldNotSetAOWhenRunPartialAndNotLegacy(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");
            new AxeBuilder(WebDriver).Analyze();
            var allowedOrigins = AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins(browser)));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldNotSetAOWhenRunPartialAndLegacy(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");
#pragma warning disable CS0618
            new AxeBuilder(WebDriver)
                .UseLegacyMode(true)
                .Analyze();
#pragma warning restore CS0618
            var allowedOrigins = AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins(browser)));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldNotSetAOWhenLegacySourceAndLegacyMode(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");
#pragma warning disable CS0618
            new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeForceLegacy}"))
                .UseLegacyMode(true)
                .Analyze();
#pragma warning restore CS0618
            var allowedOrigins = AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins(browser)));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldSetAOWhenLegacySourceAndNotLegacyMode(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");
            new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeForceLegacy}"))
                .Analyze();
            var allowedOrigins = AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo("*"));
        }

        [Test]
        [TestCase("Chrome")]
        // The cross-orgin test has an iframe that 404s. FireFox is reporting that
        // frame as `readyState = 'interactive'`. This breaks our "AssertFrameReady"
        // code.
        public void ShouldBeDisabledAgain(string browser)
        {
            InitDriver(browser);
            GoToFixture("cross-origin.html");
            // GoToUrl("http://localhost:8080/cross-origin.html");

#pragma warning disable CS0618
            var results = new AxeBuilder(WebDriver)
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .UseLegacyMode(false)
                .Analyze();
#pragma warning restore CS0618

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNull(frameTested);
        }

        private string AllowedOrigins()
        {
            return (string)((IJavaScriptExecutor)WebDriver).ExecuteScript("return axe._audit.allowedOrigins[0]");
        }

        private string ExpectedAllowedOrigins(string browser)
        {
            switch (browser.ToUpper())
            {
                case "CHROME":
                    return "http://localhost:8080";

                case "FIREFOX":
                    return "http://localhost:8080";
                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");
            }
        }

    }
}
