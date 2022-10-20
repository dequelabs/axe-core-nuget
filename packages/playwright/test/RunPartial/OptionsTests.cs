using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Deque.AxeCore.Commons;
using NUnit.Framework;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{
    public class OptionsTests : TestBase
    {
        protected const string RULE_TO_TEST = "region";

        [Test]
        public async Task ShouldPassOptionsToAxeCore()
        {
            await GoToFixture("index.html");

            var rules = new Dictionary<string, RuleOptions>();
            rules.Add(RULE_TO_TEST, new RuleOptions
            {
                Enabled = true
            });

            var results = await Page!.RunAxe(new AxeRunOptions
            {
                Rules = rules
            });

            var combinedRules = new List<AxeResultItem>();
            combinedRules.AddRange(results.Passes);
            combinedRules.AddRange(results.Inapplicable);
            combinedRules.AddRange(results.Incomplete);
            combinedRules.AddRange(results.Violations);

            Assert.IsTrue(combinedRules.Any(x => x.Id == RULE_TO_TEST));
        }

        [Test]
        public async Task ShouldOnlyRunSpecifiedRule()
        {
            await GoToFixture("index.html");

            var rules = new Dictionary<string, RuleOptions>();
            rules.Add(RULE_TO_TEST, new RuleOptions
            {
                Enabled = false
            });

            var results = await Page!.RunAxe(new AxeRunOptions
            {
                Rules = rules
            });
            Assert.IsTrue(
                !results.Passes.Any(x => x.Id == RULE_TO_TEST)
                && !results.Inapplicable.Any(x => x.Id == RULE_TO_TEST)
                && !results.Incomplete.Any(x => x.Id == RULE_TO_TEST)
                && !results.Violations.Any(x => x.Id == RULE_TO_TEST)
            );
        }
    }
}
