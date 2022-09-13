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
        public void ShouldAnalyzeNestedIFrames()
        {
            GoToFixture("nested-iframes.html");

            var res = new AxeBuilder(driver)
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
        public void ShouldAnalyzeNestedFramesets()
        {
            GoToFixture("nested-frameset.html");

            var res = new AxeBuilder(driver)
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
        public void ShouldAnalyzeShadowDOMFrames()
        {
            GoToFixture("shadow-frames.html");

            var res = new AxeBuilder(driver)
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
        public void ShouldReportErroringFrames()
        {
            GoToFixture("crash-parent.html");

            var res = new AxeBuilder(driver, CustomSource($"{axeSource}{axeCrashScript}"))
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
