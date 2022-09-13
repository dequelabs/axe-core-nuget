using System.Linq;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class UseLegacyTests : TestBase
    {
        [Test]
        public void ShouldRunLegacyAxe()
        {
            GoToFixture("index.html");

            var results = new AxeBuilder(driver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .Analyze();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
        }

        [Test]
        public void ShouldNotTestCrossOriginFrames()
        {
            GoToUrl("http://localhost:8080/cross-origin.html");

            var results = new AxeBuilder(driver, CustomSource($"{axeSource}{axeRunPartialThrows}"))
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .Analyze();

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNotNull(frameTested);
        }

        [Test]
        public void ShouldBeDisabledAgain()
        {
            GoToUrl("http://localhost:8080/cross-origin.html");

            var results = new AxeBuilder(driver)
                .UseLegacyMode(true)
                .WithRules("frame-tested")
                .UseLegacyMode(false)
                .Analyze();

            var frameTested = results.Incomplete.FirstOrDefault(x => x.Id == "frame-tested");
            Assert.IsNull(frameTested);
        }
    }
}
