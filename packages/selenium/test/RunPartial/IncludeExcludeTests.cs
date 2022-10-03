using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class IncludeExcludeTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHonorInclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Include(".include");
            var includeRes = axe.Analyze();

            var axe2 = new AxeBuilder(WebDriver);
            var normalRes = axe2.Analyze();
            Assert.That(includeRes.Violations.Length, Is.LessThan(normalRes.Violations.Length));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHonorExclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Exclude(".exclude");
            var excludeRes = axe.Analyze();

            var axe2 = new AxeBuilder(WebDriver);
            var normalRes = axe2.Analyze();
            Assert.That(excludeRes.Violations.Length, Is.LessThan(normalRes.Violations.Length));
        }
    }
}
