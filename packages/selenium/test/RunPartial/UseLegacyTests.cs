using System.Linq;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class UseLegacyTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        public void ShouldRunLegacyAxe(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var results = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .Analyze();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldNotTestCrossOriginFrames(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/cross-origin.html");

            var results = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .Analyze();

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNotNull(frameTested);
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldBeDisabledAgain(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/cross-origin.html");

            var results = new AxeBuilder(WebDriver)
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .UseLegacyMode(false)
                .Analyze();

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNull(frameTested);
        }
    }
}
