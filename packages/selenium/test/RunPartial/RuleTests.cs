using System;
using System.Collections.Generic;
using System.Linq;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class RuleTests : TestBase
    {
        protected const string RULE_TO_TEST = "region";

        [Test]
        [TestCase("Chrome")]
        public void ShouldDisableGivenRules(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var res = new AxeBuilder(WebDriver)
                .DisableRules(RULE_TO_TEST)
                .Analyze();

            var rules = new List<AxeResultItem>();
            rules.AddRange(res.Passes);
            rules.AddRange(res.Inapplicable);
            rules.AddRange(res.Incomplete);
            rules.AddRange(res.Violations);

            Assert.IsTrue(!rules.Any(x => x.Id == RULE_TO_TEST));
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldRunOnlyGivenRules(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var res = new AxeBuilder(WebDriver)
                .WithRules(RULE_TO_TEST)
                .Analyze();

            var rules = new List<AxeResultItem>();
            rules.AddRange(res.Passes);
            rules.AddRange(res.Inapplicable);
            rules.AddRange(res.Incomplete);
            rules.AddRange(res.Violations);

            Assert.IsTrue(rules.All(x => x.Id == RULE_TO_TEST));
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldAllowWithRulesMultipleTimes(string browser)
        {
            InitDriver(browser);
            Assert.DoesNotThrow(() =>
            {
                new AxeBuilder(WebDriver)
                    .WithRules(RULE_TO_TEST)
                    .WithRules("label");
            });
        }
    }
}
