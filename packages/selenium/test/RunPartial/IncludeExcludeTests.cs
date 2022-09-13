using System;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class IncludeExcludeTests : TestBase
    {
        [Test]
        public void ShouldNotThrowWithIncludeExclude()
        {
            GoToResource("context.html");

            var axe = new AxeBuilder(driver)
                .Include(".include")
                .Exclude(".exclude");
            Assert.DoesNotThrow(() => axe.Analyze());
        }

        [Test]
        public void ShouldNotThrowWithInclude()
        {
            GoToResource("context.html");

            var axe = new AxeBuilder(driver)
                .Include(".include");
            Assert.DoesNotThrow(() => axe.Analyze());
        }

        [Test]
        public void ShouldNotThrowWithExclude()
        {
            GoToResource("context.html");

            var axe = new AxeBuilder(driver)
                .Exclude(".exclude");
            Assert.DoesNotThrow(() => axe.Analyze());
        }
    }
}
