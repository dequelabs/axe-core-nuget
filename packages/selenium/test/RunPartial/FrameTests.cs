using System.Collections.Generic;
using System.Linq;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

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
    }
}
