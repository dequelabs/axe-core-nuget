using System;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class IncludeExcludeTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        public void ShouldNotThrowWithIncludeExclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Include(".include")
                .Exclude(".exclude");
            Assert.DoesNotThrow(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldNotThrowWithInclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Include(".include");
            Assert.DoesNotThrow(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldNotThrowWithExclude(string browser)
        {
            InitDriver(browser);
            GoToResource("context.html");

            var axe = new AxeBuilder(WebDriver)
                .Exclude(".exclude");
            Assert.DoesNotThrow(() => axe.Analyze());
        }
    }
}
