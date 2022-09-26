using System.Linq;
using System.IO;
using Deque.AxeCore.Commons;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class FinishRunTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldIsolateCallToFinishRun(string browser)
        {
            InitDriver(browser);
            GoToFixture("isolated-finish.html");
            var axe = new AxeBuilder(WebDriver);
            Assert.DoesNotThrow(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldHaveSameResultsAsLegacy(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-iframes.html");

            var legacyResults = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeForceLegacy}"))
                .Analyze();

            Assert.That(legacyResults.TestEngineName, Is.EqualTo("axe-legacy"));


            GoToFixture("nested-iframes.html");
            var runPartialResults = new AxeBuilder(WebDriver)
                .Analyze();

            Assert.That(legacyResults.Violations.Length, Is.EqualTo(runPartialResults.Violations.Length));
            for (int i = 0; i < legacyResults.Violations.Length; i++)
            {
                AssertAxeItemsEqual(legacyResults.Violations[i], runPartialResults.Violations[i], true);
            }

            Assert.That(legacyResults.Passes.Length, Is.EqualTo(runPartialResults.Passes.Length));
            for (int i = 0; i < runPartialResults.Passes.Length; i++)
            {
                AssertAxeItemsEqual(legacyResults.Passes[i], runPartialResults.Passes[i]);
            }
            Assert.That(legacyResults.Inapplicable.Length, Is.EqualTo(runPartialResults.Inapplicable.Length));
            for (int i = 0; i < legacyResults.Inapplicable.Length; i++)
            {
                AssertAxeItemsEqual(legacyResults.Inapplicable[i], runPartialResults.Inapplicable[i]);
            }
            Assert.That(legacyResults.Incomplete.Length, Is.EqualTo(runPartialResults.Incomplete.Length));
            for (int i = 0; i < legacyResults.Incomplete.Length; i++)
            {
                AssertAxeItemsEqual(legacyResults.Incomplete[i], runPartialResults.Incomplete[i]);
            }
            Assert.That(ToJson(legacyResults.TestEnvironment), Is.EqualTo(ToJson(runPartialResults.TestEnvironment)));
            Assert.That(ToJson(legacyResults.TestRunner), Is.EqualTo(ToJson(runPartialResults.TestRunner)));
            Assert.That(ToJson(legacyResults.Url), Is.EqualTo(ToJson(runPartialResults.Url)));
            Assert.That(ToJson(legacyResults.Error), Is.EqualTo(ToJson(runPartialResults.Error)));
            Assert.That(ToJson(legacyResults.TestEngineVersion), Is.EqualTo(ToJson(runPartialResults.TestEngineVersion)));
            Assert.That(ToJson(legacyResults.ToolOptions), Is.EqualTo(ToJson(runPartialResults.ToolOptions)));
        }

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include
        };

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

        private static void AssertAxeItemsEqual(AxeResultItem a, AxeResultItem b, bool compareNodes = false)
        {
            Assert.That(a.Id, Is.EqualTo(b.Id));
            Assert.That(a.Description, Is.EqualTo(b.Description));
            Assert.That(a.Help, Is.EqualTo(b.Help));
            Assert.That(a.HelpUrl, Is.EqualTo(b.HelpUrl));
            Assert.That(a.Impact, Is.EqualTo(b.Impact));
            Assert.That(ToJson(a.Tags.OrderBy(x => x)), Is.EqualTo(ToJson(b.Tags.OrderBy(x => x))));

            if (compareNodes)
            {
                Assert.That(a.Nodes.Length, Is.EqualTo(b.Nodes.Length));
            }
        }
    }
}
