using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{
    public class FrameTests : TestBase
    {
        [Test]
        public async Task ShouldAnalyzeNestedIFrames()
        {
            await GoToFixture("nested-iframes.html");

            var res = await Page!.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(4));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldAnalyzeNestedFramesets()
        {
            await GoToFixture("nested-frameset.html");

            var res = await Page!.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(4));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldAnalyzeShadowDOMFrames()
        {
            await GoToFixture("shadow-frames.html");

            var res = await Page!.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(3));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldReportErroringFrames()
        {
            await GoToFixture("crash-parent.html");

            var res = await RunWithCustomAxe(CustomSource($"{{ {axeSource}; {axeCrashScript}; }}"));
            bool found = false;
            foreach (var rule in res.Violations)
            {
                if (rule.Id == "label")
                {
                    found = true;
                    Assert.That(rule.Nodes.Count, Is.EqualTo(2));

                }
            }
            Assert.That(found, Is.EqualTo(true));

            found = false;
            foreach (var rule in res.Incomplete)
            {
                if (rule.Id == "frame-tested")
                {
                    found = true;
                    Assert.That(rule.Nodes.Count, Is.EqualTo(1));

                }
            }
            Assert.That(found, Is.EqualTo(true));
        }
    }
}
