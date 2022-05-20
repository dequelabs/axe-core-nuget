#nullable enable

using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Playwright.Axe.Test
{
    [TestClass]
    public class IntegrationTests : PageTest
    {
        private readonly TestServer m_testServer;

        public IntegrationTests()
        {
#if DEBUG
            Environment.SetEnvironmentVariable("HEADED", "1");
#endif
            m_testServer = new();
        }

        [TestInitialize]
        public async Task InitializeTest()
        {
            await m_testServer.StartAsync();
        }

        [TestCleanup]
        public async Task CleanupTest()
        {
            await m_testServer.StopAsync();
        }

        [TestMethod]
        [DynamicData(nameof(GetAxeRulesParameters), DynamicDataSourceType.Method)]
        public async Task GetAxeRules_WithTags_ReturnsAxeRules(IList<string>? tags)
        {
            await NavigateToPage("basic.html");
            IList<AxeRuleMetadata> axeRules = await Page!.GetAxeRules(tags);

            AxeRuleMetadata? rule = axeRules.FirstOrDefault();
            Assert.IsNotNull(rule);
            Assert.IsNotNull(rule.RuleId);
            Assert.IsNotNull(rule.Description);
            Assert.IsNotNull(rule.Help);
            Assert.IsNotNull(rule.HelpUrl);

            if (tags != null)
            {
                Assert.IsTrue(axeRules.All(axeRule => tags.Any(tag => axeRule.Tags.Contains(tag))));
            }
        }

        [TestMethod]
        public async Task RunAxe_NoOptions()
        {
            const string expectedViolationId = "color-contrast";

            await NavigateToPage("basic.html");

            AxeResults axeResults = await Page!.RunAxe();
            AxeResult violation = axeResults.Violations.First();

            Assert.AreEqual(violation.Id, expectedViolationId);
            Assert.AreEqual(violation.Impact!.Value, AxeImpactValue.Serious);
            Assert.IsFalse(string.IsNullOrEmpty(violation.Description));
            Assert.IsFalse(string.IsNullOrEmpty(violation.Help));
        }

        [TestMethod]
        [DynamicData(nameof(GetRunsOnlyParameters), DynamicDataSourceType.Method)]
        public async Task RunAxe_WithRunOnly_RunsOnlySpecified(IList<string> values, AxeRunOnlyType type, Func<AxeResults, IList<string>, bool> validator)
        {
            await NavigateToPage("basic.html");

            AxeRunOnly runOnly = new(type, values);
            AxeRunOptions options = new(runOnly);

            AxeResults axeResults = await Page!.RunAxe(options);
            Assert.IsTrue(validator(axeResults, values));
        }

        [TestMethod]
        public async Task RunAxe_WithRules_RunsOnlySpecified()
        {
            await NavigateToPage("basic.html");
            const string ruleId = "color-contrast";

            IDictionary<string, AxeRuleObjectValue> rules = new Dictionary<string, AxeRuleObjectValue>()
            {
                { ruleId, new AxeRuleObjectValue(false) }
            };
            AxeRunOptions options = new(rules: rules);

            AxeResults axeResults = await Page!.RunAxe(options);
            Assert.IsFalse(axeResults.Passes.Any(result => result.Id.Equals(ruleId)));
            Assert.IsFalse(axeResults.Violations.Any(result => result.Id.Equals(ruleId)));
            Assert.IsFalse(axeResults.Incomplete.Any(result => result.Id.Equals(ruleId)));
            Assert.IsFalse(axeResults.Inapplicable.Any(result => result.Id.Equals(ruleId)));
        }

        [TestMethod]
        public async Task RunAxe_WithResultTypes_RunsOnlySpecified()
        {
            await NavigateToPage("basic.html");

            IList<AxeResultGroup> resultGroups = new List<AxeResultGroup>()
            {
                AxeResultGroup.Passes 
            };

            AxeRunOptions options = new(resultTypes: resultGroups);
            
            AxeResults axeResults = await Page!.RunAxe(options);      
            Assert.AreEqual(1, axeResults.Violations.First(v => v.Id.Equals("color-contrast")).Nodes!.Count, 
                "There are two color-contrast issues, but the Nodes should be capped at 1.");
            Assert.IsTrue(axeResults.Passes.First(v => v.Id.Equals("region")).Nodes!.Count > 1);
        }

        [TestMethod]
        [DynamicData(nameof(GetRemainingRunOptionsParameters), DynamicDataSourceType.Method)]
        public async Task RunAxe_WithRunOptions_ValidatorHolds(
            AxeRunOptions runOptions, 
            Func<AxeResults, bool> validator,
            string testFile)
        {
            await NavigateToPage(testFile);

            AxeResults axeResults = await Page!.RunAxe(runOptions);
            Assert.IsTrue(validator(axeResults));
        }

        [TestMethod]
        public async Task RunAxe_NestedIframes_ExecutesSuccessfully()
        {
            const string expectedViolationId = "aria-roles";
            const string expectedViolationTarget = "#div-fail";

            await NavigateToPage("with-frame.html");

            AxeResults axeResults = await Page!.RunAxe();

            Assert.AreEqual(1, axeResults.Violations.Count);
            AxeResult ariaViolation = axeResults.Violations.First();
            IList<string> targets = ariaViolation.Nodes!.First().Target!;

            Assert.AreEqual(ariaViolation.Id, expectedViolationId);
            Assert.IsTrue(targets.Any(target => target.Equals(expectedViolationTarget)));
        }

        [TestMethod]
        [DataRow("button")]
        [DataRow("text=Text example")]
        [DataRow("[aria-label='Accessibility Label']")]
        [DataRow("id=id-example")]
        public async Task RunAxeOnLocator_NoOptions(string selector)
        {
            await NavigateToPage("selector.html");

            AxeResults axeResults = await Page!.Locator(selector).RunAxe();
            Assert.IsTrue(axeResults.Passes.All(pass => pass.Nodes!.Count == 1));
        }

        [TestMethod]
        public async Task RunAxeOnLocator_WithOptions()
        {
            const string tag = "ACT";
            await NavigateToPage("selector.html");

            AxeRunOptions runOptions = new(new AxeRunOnly(AxeRunOnlyType.Tag, new List<string> { tag }));

            AxeResults axeResults = await Page!.Locator("button").RunAxe(runOptions);
            Assert.IsTrue(axeResults.Passes.All(pass => pass.Tags!.Contains(tag)));
        }

        [TestMethod]
        [DynamicData(nameof(GetAxeContextParameters), DynamicDataSourceType.Method)]
        public async Task RunAxe_WithContext(AxeRunContext axeRunContext, ISet<string> targets, bool includedInTargets)
        {
            await NavigateToPage("selector.html");

            AxeResults axeResults = await Page!.RunAxe(axeRunContext);

            Assert.IsTrue(axeResults.Passes
                .All(pass => pass.Nodes!
                .All(node => node.Target!
                .Any(target => includedInTargets == targets.Contains(target)))));
        }

        [TestMethod]
        [DynamicData(nameof(GetReportParameters), DynamicDataSourceType.Method)]
        public async Task RunAxe_WithReport_OutputsReport(AxeHtmlReportOptions reportOptions, string? expectedReportPath)
        {
            await NavigateToPage("basic.html");

            await Page!.RunAxe(reportOptions: reportOptions);

            Assert.IsTrue(File.Exists(expectedReportPath));
        }   

        private static IEnumerable<object?[]> GetAxeRulesParameters()
        {
            yield return new object?[]
            {
                null
            };

            yield return new object?[]
            {
                new List<string>()
                {
                    "wcag2aa",
                    "wcag2a"
                }
            };
        }
        private static IEnumerable<object?[]> GetRunsOnlyParameters()
        {
            Func<AxeResults, IList<string>, bool> f0 = (AxeResults results, IList<string> values) => results.Violations
                .All(violation => violation.Tags!.Any(tag => values.Contains(tag)));
            yield return new object?[]
            {
                new List<string> { "wcag2a" },
                AxeRunOnlyType.Tag,
                f0
            };

            Func<AxeResults, IList<string>, bool> f1 = (AxeResults results, IList<string> values) => results.Inapplicable
                .All(inapplicable => values.Contains(inapplicable.Id));
            yield return new object?[]
            {
                new List<string> { "color-contrast" },
                AxeRunOnlyType.Rule,
                f1
            };
        }
    
        private static IEnumerable<object?[]> GetRemainingRunOptionsParameters()
        {
            Func<AxeResults, bool> selectorValidator = (AxeResults results) => 
                results.Violations
                .All(violation => violation.Nodes!
                .All(node => node.Target == null));
            yield return new object?[]
            {
                new AxeRunOptions(selectors: false),
                selectorValidator,
                "basic.html"
            };

            Func<AxeResults, bool> ancestorValidator = (AxeResults results) => results.Violations
                .Any(violation => violation.Nodes!
                .Any(node => node.Ancestry != null && node.Ancestry.Any())); 
            yield return new object?[]
            {
                new AxeRunOptions(ancestry: true),
                ancestorValidator,
                "basic.html"
            };

            Func<AxeResults, bool> xpathValidator = (AxeResults results) => results.Violations
                .Any(violation => violation.Nodes!
                .Any(node => node.XPath != null && node.XPath.Any()));
            yield return new object?[]
            {
                new AxeRunOptions(xpath: true),
                xpathValidator,
                "basic.html"
            };

            Func<AxeResults, bool> iframeValidator = (AxeResults results) => true;
            yield return new object?[]
            {
                new AxeRunOptions(iframes: true),
                iframeValidator,
                "with-frame.html"
            };
        }

        private static IEnumerable<object?[]> GetAxeContextParameters()
        {
            yield return new object?[]
            {
                new AxeRunSerialContext("#id-example"),
                new HashSet<string>()
                {
                    "#id-example"
                },
                true
            };

            yield return new object?[]
            {
                new AxeRunSerialContext("a"),
                new HashSet<string>()
                {
                    "a[aria-label=\"Accessibility Label\"]",
                    "#id-example"
                },
                true
            };

            yield return new object?[]
            {
                new AxeRunSerialContext(null, "#id-example"),
                new HashSet<string>()
                {
                    "#id-example"
                },
                false
            };

            yield return new object?[]
            {
                new AxeRunSerialContext(null, "a"),
                new HashSet<string>()
                {
                    "a[aria-label=\"Accessibility Label\"]",
                    "#id-example"
                },
                false
            };
        }

        private static IEnumerable<object?[]> GetReportParameters()
        {
            yield return new object[]
            {
                new AxeHtmlReportOptions(),
                "report/index.html"
            };
        }

        private async Task NavigateToPage(string htmlPageName)
        {
            Uri uri = new(TestServer.BaseUri, htmlPageName);
            await Page!.GotoAsync(uri.ToString());
        }
    }
}
