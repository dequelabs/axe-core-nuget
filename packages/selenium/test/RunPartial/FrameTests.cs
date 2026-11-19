using System;
using System.Diagnostics;
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
        public void ShouldInjectIntoNestedIFramesByDefault(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-iframes.html");

            new AxeBuilder(WebDriver)
                .WithRules("label")
                .Analyze();

            AssertAxeInjectedIntoChildFrames();
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldNotInjectIntoNestedIFramesWhenIframesIsFalse(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-iframes.html");

            var res = new AxeBuilder(WebDriver)
                .WithOptions(LabelOnlyWithoutIframes())
                .Analyze();

            Assert.That(res.Violations, Is.Empty);
            AssertAxeNotInjectedIntoChildFrames();
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
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldReportFramesSandboxedWithoutScripts(string browser)
        {
            InitDriver(browser);
            GoToUrl($"{fixtureServer.BaseUrl}/index.html");
            AddFrame("no-scripts", "/index.html", "allow-same-origin");

            var res = AnalyzeFrameTestedQuickly();

            AssertBlockedFrame(res, browser, "#no-scripts");
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldReportFramesSandboxedByCspHeader(string browser)
        {
            InitDriver(browser);
            GoToUrl($"{fixtureServer.BaseUrl}/index.html");
            AddFrame("csp-sandbox", "/csp-sandbox/index.html", null);

            var res = AnalyzeFrameTestedQuickly();

            AssertBlockedFrame(res, browser, "#csp-sandbox");
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldUseTheSandboxAFrameLoadedWith(string browser)
        {
            InitDriver(browser);
            GoToUrl($"{fixtureServer.BaseUrl}/index.html");
            AddFrame("sandbox-added-after-load", "/index.html", null);
            AddFrame("sandbox-removed-after-load", "/index.html", "allow-same-origin");
            ((IJavaScriptExecutor)WebDriver).ExecuteScript(@"
                document.getElementById('sandbox-added-after-load').setAttribute('sandbox', 'allow-same-origin');
                document.getElementById('sandbox-removed-after-load').removeAttribute('sandbox');
            ");

            var res = AnalyzeFrameTestedQuickly();

            Assert.That(FrameTestedTargets(res.Passes), Does.Contain("#sandbox-added-after-load"));
            AssertBlockedFrame(res, browser, "#sandbox-removed-after-load");
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldAnalyzeFramesSandboxedWithScripts(string browser)
        {
            InitDriver(browser);
            GoToUrl($"{fixtureServer.BaseUrl}/index.html");
            AddFrame("scripts", "/index.html", "allow-same-origin allow-scripts");

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#scripts" }));
            Assert.That(res.Incomplete, Is.Empty);
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldScanPagesAndFramesThatReplaceEvent(string browser)
        {
            InitDriver(browser);
            GoToUrl($"{fixtureServer.BaseUrl}/index.html");
            ((IJavaScriptExecutor)WebDriver).ExecuteScript("window.Event = function () {};");
            AddFrame("replaced-event", "/index.html", null);
            ((IJavaScriptExecutor)WebDriver).ExecuteScript(
                "document.getElementById('replaced-event').contentWindow.Event = function () {};");

            var res = new AxeBuilder(WebDriver)
                .WithRules("frame-tested")
                .Analyze();

            Assert.That(FrameTestedTargets(res.Passes), Is.EqualTo(new[] { "#replaced-event" }));
            Assert.That(FrameTestedTargets(res.Incomplete), Is.Empty);
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

        private AxeResult AnalyzeFrameTestedQuickly()
        {
            var builder = new AxeBuilder(WebDriver).WithRules("frame-tested");
            var stopwatch = Stopwatch.StartNew();
            var res = builder.Analyze();
            stopwatch.Stop();
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(10)));
            return res;
        }

        private static void AssertBlockedFrame(AxeResult res, string browser, string target)
        {
            var scanned = browser == "Firefox";
            Assert.That(FrameTestedTargets(scanned ? res.Passes : res.Incomplete), Does.Contain(target));
            Assert.That(FrameTestedTargets(scanned ? res.Incomplete : res.Passes), Does.Not.Contain(target));
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
