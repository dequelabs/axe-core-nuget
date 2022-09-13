using System.Collections.Generic;
using System.Linq;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class OptionsTests : TestBase
    {
        protected const string RULE_TO_TEST = "region";

        [Test]
        public void ShouldPassOptionsToAxeCore()
        {
            GoToFixture("index.html");

            var rules = new Dictionary<string, RuleOptions>();
            rules.Add(RULE_TO_TEST, new RuleOptions
            {
                Enabled = true
            });

            var results = new AxeBuilder(driver)
                .WithOptions(new AxeRunOptions
                {
                    Rules = rules
                })
                .Analyze();

            var combinedRules = new List<AxeResultItem>();
            combinedRules.AddRange(results.Passes);
            combinedRules.AddRange(results.Inapplicable);
            combinedRules.AddRange(results.Incomplete);
            combinedRules.AddRange(results.Violations);

            Assert.IsTrue(combinedRules.Any(x => x.Id == RULE_TO_TEST));
        }

        [Test]
        public void ShouldOnlyRunSpecifiedRule()
        {
            GoToFixture("index.html");

            var results = new AxeBuilder(driver)
                .DisableRules(RULE_TO_TEST)
                .Analyze();

            Assert.IsTrue(
                !results.Passes.Any(x => x.Id == RULE_TO_TEST)
                && !results.Inapplicable.Any(x => x.Id == RULE_TO_TEST)
                && !results.Incomplete.Any(x => x.Id == RULE_TO_TEST)
                && !results.Violations.Any(x => x.Id == RULE_TO_TEST)
            );
        }
    }
}
