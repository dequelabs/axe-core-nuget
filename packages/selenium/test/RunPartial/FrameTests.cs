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
        public void ShouldAnalyzeNestedIFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-iframes.html");

            var res = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions
                {
                    RunOnly = RunOnlyOptions.Rules("label")
                })
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(4));
            // todo: check for frame ids
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldAnalyzeNestedFramesets(string browser)
        {
            InitDriver(browser);
            GoToFixture("nested-frameset.html");

            var res = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions
                {
                    RunOnly = RunOnlyOptions.Rules("label")
                })
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(4));
            // todo: check for frame ids
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldAnalyzeShadowDOMFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("shadow-frames.html");

            var res = new AxeBuilder(WebDriver)
                .WithOptions(new AxeRunOptions
                {
                    RunOnly = RunOnlyOptions.Rules("label")
                })
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(3));
            // todo: check for frame ids
        }

        [Test]
        [TestCase("Chrome")]
        public void ShouldReportErroringFrames(string browser)
        {
            InitDriver(browser);
            GoToFixture("crash-parent.html");

            var res = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{axeCrashScript}"))
                .WithOptions(new AxeRunOptions
                {
                    RunOnly = RunOnlyOptions.Rules("label", "frame-tested")
                })
                .Analyze();

            var violation = res.Violations.FirstOrDefault();
            var incomplete = res.Incomplete.FirstOrDefault();
            Assert.IsNotNull(violation);
            Assert.That(violation.Id, Is.EqualTo("label"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(2));
            // todo: check for frame ids
            Assert.IsNotNull(incomplete);
            Assert.That(incomplete.Id, Is.EqualTo("frame-tested"));
            Assert.That(violation.Nodes.Length, Is.EqualTo(2));
            // todo: check for frame ids
        }
    }
}
