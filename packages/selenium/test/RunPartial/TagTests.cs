using System;
using System.Collections.Generic;
using System.Linq;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class TagTests : TestBase
    {
        protected const string TAG_TO_TEST = "best-practice";

        [Test]
        [TestCase("Chrome")]
        public void ShouldOnlyRunRulesWithGivenTag(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var results = new AxeBuilder(WebDriver)
                .WithTags(TAG_TO_TEST)
                .Analyze();

            var rules = new List<AxeResultItem>();
            rules.AddRange(results.Passes);
            rules.AddRange(results.Inapplicable);
            rules.AddRange(results.Incomplete);
            rules.AddRange(results.Violations);

            Assert.IsTrue(rules.Any(x => x.Tags.Contains(TAG_TO_TEST)));
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldAllowWithTagsMultipleTimes(string browser)
        {
            InitDriver(browser);
            Assert.DoesNotThrow(() =>
            {
                new AxeBuilder(WebDriver)
                    .WithTags(TAG_TO_TEST)
                    .WithTags("wcag2a");
            });
        }
    }
}
