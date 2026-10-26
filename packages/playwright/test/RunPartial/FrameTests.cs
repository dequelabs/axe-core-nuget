using System.Linq;
using Deque.AxeCore.Commons;
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
        public async Task ShouldAnalyzeInNestedIFrames()
        {
            await GoToFixture("nested-iframes.html");
            var frame = Page!.Locator("#ifr-foo");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(2));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldAnalyzeElementInNestedIFrames()
        {
            await GoToFixture("nested-iframes.html");
            var frame = Page!.FrameLocator("#ifr-foo").Locator("#foo-bar");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(1));
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
        public async Task ShouldAnalyzeInNestedFramesets()
        {
            await GoToFixture("nested-frameset.html");
            var frame = Page!.Locator("#frm-foo");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(2));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldAnalyzeElementInNestedFramesets()
        {
            await GoToFixture("nested-frameset.html");
            var frame = Page!.FrameLocator("#frm-foo").Locator("#foo-bar");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(1));
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
        public async Task ShouldAnalyzeInShadowDOMFrames()
        {
            await GoToFixture("shadow-frames.html");
            var frame = Page!.Locator("#shadow-root");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(2));
                    return;
                }
            }
            Assert.Fail("Could not find label violation");
        }

        [Test]
        public async Task ShouldAnalyzeElementInShadowDOMFrames()
        {
            await GoToFixture("shadow-frames.html");
            var frame = Page!.Locator("#shadow-frame");

            var res = await frame.RunAxe();

            foreach (var violation in res.Violations)
            {

                if (violation.Id == "label")
                {
                    Assert.That(violation.Nodes.Count, Is.EqualTo(1));
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

        [Test]
        [Timeout(15000)]
        public async Task ShouldReportFramesSandboxedWithoutScripts()
        {
            await GoToFixture("index.html");
            await AddFrame("no-scripts", "/index.html", "allow-same-origin");

            var res = await RunFrameTested();

            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#no-scripts" }));
        }

        [Test]
        [Timeout(15000)]
        public async Task ShouldReportFramesSandboxedByCspHeader()
        {
            await GoToFixture("index.html");
            await AddFrame("csp-sandbox", "/csp-sandbox/index.html", null);

            var res = await RunFrameTested();

            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#csp-sandbox" }));
        }

        [Test]
        [Timeout(15000)]
        public async Task ShouldUseTheSandboxAFrameLoadedWith()
        {
            await GoToFixture("index.html");
            await AddFrame("sandbox-added-after-load", "/index.html", null);
            await AddFrame("sandbox-removed-after-load", "/index.html", "allow-same-origin");
            await Page!.EvaluateAsync(@"() => {
                document.getElementById('sandbox-added-after-load').setAttribute('sandbox', 'allow-same-origin');
                document.getElementById('sandbox-removed-after-load').removeAttribute('sandbox');
            }");

            var res = await RunFrameTested();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#sandbox-added-after-load" }));
            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#sandbox-removed-after-load" }));
        }

        [Test]
        [Timeout(15000)]
        public async Task ShouldAnalyzeFramesSandboxedWithScripts()
        {
            await GoToFixture("index.html");
            await AddFrame("scripts", "/index.html", "allow-same-origin allow-scripts");

            var res = await RunFrameTested();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#scripts" }));
            Assert.That(res.Incomplete, Is.Empty);
        }

        [Test]
        [Timeout(15000)]
        public async Task ShouldScanPagesAndFramesThatReplaceEvent()
        {
            await GoToFixture("index.html");
            await Page!.EvaluateAsync("() => { window.Event = function () {}; }");
            await AddFrame("replaced-event", "/index.html", null);
            await Page!.EvaluateAsync(
                "() => { document.getElementById('replaced-event').contentWindow.Event = function () {}; }");

            var res = await RunFrameTested();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#replaced-event" }));
            Assert.That(FrameTestedTargets(res.Incomplete), Is.Empty);
        }

        private Task AddFrame(string id, string src, string sandbox)
        {
            return Page!.EvaluateAsync(@"([id, src, sandbox]) => new Promise(resolve => {
                const iframe = document.createElement('iframe');
                iframe.id = id;
                if (sandbox !== null) {
                    iframe.setAttribute('sandbox', sandbox);
                }
                iframe.onload = resolve;
                iframe.src = src;
                document.body.appendChild(iframe);
            })", new[] { id, src, sandbox });
        }

        private Task<AxeResult> RunFrameTested()
        {
            return Page!.RunAxe(new AxeRunOptions
            {
                RunOnly = RunOnlyOptions.Rules("frame-tested")
            });
        }

        private static string[] FrameTestedTargets(AxeResultItem[] items)
        {
            return items
                .Where(item => item.Id == "frame-tested")
                .SelectMany(item => item.Nodes.Select(node => node.Target.Selector))
                .ToArray();
        }
    }
}
