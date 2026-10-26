using System.Linq;
using Deque.AxeCore.Commons;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class FrameTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldAnalyzeNestedIFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-iframes.html");

            var res = new AxeBuilder(WebDriver)
                .WithRules("label")
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(4));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldAnalyzeNestedFramesets(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-frameset.html");

            var res = new AxeBuilder(WebDriver)
                .WithRules("label")
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(4));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldAnalyzeShadowDOMFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("shadow-frames.html");

            var res = new AxeBuilder(WebDriver)
                .WithRules("label")
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(3));
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldReportErroringFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("crash-parent.html");

            var res = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeCrashScript}"))
                .WithRules("label", "frame-tested")
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            var incomplete = res.Incomplete.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(2));
            Assert.IsNotNull(incomplete);
            Assert.That(incomplete.Id, Is.EqualTo("frame-tested"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(2));
        }

        [Test]
        [Timeout(15000)]
        [TestCase("Chrome")]
        public void ShouldReportFramesSandboxedWithoutScripts(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/index.html");
            AddFrame("no-scripts", "/index.html", "allow-same-origin");

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#no-scripts" }));
        }

        [Test]
        [Timeout(15000)]
        [TestCase("Chrome")]
        public void ShouldReportFramesSandboxedByCspHeader(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/index.html");
            AddFrame("csp-sandbox", "/csp-sandbox/index.html", null);

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#csp-sandbox" }));
        }

        [Test]
        [Timeout(15000)]
        [TestCase("Chrome")]
        public void ShouldUseTheSandboxAFrameLoadedWith(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/index.html");
            AddFrame("sandboxed-later", "/index.html", null);
            AddFrame("unsandboxed-later", "/index.html", "allow-same-origin");
            ((IJavaScriptExecutor)WebDriver).ExecuteScript(@"
                document.getElementById('sandboxed-later').setAttribute('sandbox', 'allow-same-origin');
                document.getElementById('unsandboxed-later').removeAttribute('sandbox');
            ");

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#sandboxed-later" }));
            Assert.That(FrameTestedTargets(res.Incomplete), Is.EqualTo(new[] { "#unsandboxed-later" }));
        }

        [Test]
        [Timeout(15000)]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldAnalyzeFramesSandboxedWithScripts(string browser)
        {
            InitDriver(browser);
            GoToUrl("http://localhost:8080/index.html");
            AddFrame("scripts", "/index.html", "allow-same-origin allow-scripts");

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#scripts" }));
            Assert.That(res.Incomplete, Is.Empty);
        }

        private void AddFrame(string id, string src, string sandbox)
        {
            ((IJavaScriptExecutor)WebDriver).ExecuteAsyncScript(@"
                var done = arguments[arguments.length - 1];
                var iframe = document.createElement('iframe');
                iframe.id = arguments[0];
                if (arguments[2] !== null) {
                    iframe.setAttribute('sandbox', arguments[2]);
                }
                iframe.onload = function () { done(); };
                iframe.src = arguments[1];
                document.body.appendChild(iframe);
            ", id, src, sandbox);
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
