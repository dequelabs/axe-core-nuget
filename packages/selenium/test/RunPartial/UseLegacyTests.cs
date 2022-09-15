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

#pragma warning disable CS0618
            var results = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .Analyze();
#pragma warning restore CS0618

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldNotTestCrossOriginFrames(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/cross-origin.html");

#pragma warning disable CS0618
            var results = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .Analyze();
#pragma warning restore CS0618

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNotNull(frameTested);
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldBeDisabledAgain(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/cross-origin.html");

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
    }
}
