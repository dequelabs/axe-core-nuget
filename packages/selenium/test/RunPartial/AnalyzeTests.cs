using System;
using OpenQA.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class AnalyzeTests : TestBase
    {
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldReturnResults(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var res = new AxeBuilder(WebDriver).Analyze();
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Inapplicable);
            Assert.IsNotNull(res.Incomplete);
            Assert.IsNotNull(res.Passes);
            Assert.IsNotNull(res.Violations);
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void shouldWorkWithUnloadedIframe(string browser)
        {
            InitDriver(browser);
            GoToAxeFixture("lazy-loaded-iframe.html");

            var res = new AxeBuilder(WebDriver).WithRules("label", "frame-tested").Analyze();

            Assert.IsNotNull(res);
            Assert.That(WebDriver.Title, Is.Not.EqualTo("Error"));
            Assert.Greater(res.Incomplete.Length, 0);
            Assert.That(res.Incomplete[0].Id, Is.EqualTo("frame-tested"));
            Assert.That(res.Incomplete[0].Nodes.Length, Is.EqualTo(1));
            AssertTargetEquals(new[] { "#ifr-lazy", "#lazy-iframe" }, res.Incomplete[0].Nodes[0].Target);
            Assert.That(res.Violations[0].Id, Is.EqualTo("label"));
            Assert.That(res.Violations[0].Nodes.Length, Is.EqualTo(1));
            AssertTargetEquals(new[] { "#ifr-lazy", "#lazy-baz", "input" }, res.Violations[0].Nodes[0].Target);
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldRevertTimeout(string browser)
        {
            InitDriver(browser);
            GoToFixture("lazy-loaded-iframe.html");

            var setTO = TimeSpan.FromSeconds(50.0);
            WebDriver.Manage().Timeouts().PageLoad = setTO;
            new AxeBuilder(WebDriver).Analyze();
            Assert.That(WebDriver.Manage().Timeouts().PageLoad, Is.EqualTo(setTO));
        }


        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldFailWhenPageIsNotReady(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");
            var jsExec = (IJavaScriptExecutor)WebDriver;
            var overrideDocReady = "Object.defineProperty(document, 'readyState', {get() {return 'nope'}})";
            jsExec.ExecuteScript(overrideDocReady);
            Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains("not ready"),
                          () => new AxeBuilder(WebDriver).Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldThrowIfTopFrameErrors(string browser)
        {
            InitDriver(browser);
            GoToFixture("crash.html");

            var axe = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{Environment.NewLine}{axeCrashScript}"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldThrowWhenInjectingProblematicSource(string browser)
        {
            InitDriver(browser);
            GoToFixture("crash.html");

            var axe = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{Environment.NewLine}; throw new Error('Boom!')"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldThrowWhenSetupFails(string browser)
        {
            InitDriver(browser);
            GoToFixture("crash.html");

            var axe = new AxeBuilder(WebDriver, CustomSource($"{axeSource}{Environment.NewLine}; window.axe.utils = {{}}"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void ShouldReturnCorrectMetadata(string browser)
        {
            InitDriver(browser);
            GoToFixture("index.html");

            var res = new AxeBuilder(WebDriver)
                .Analyze();

            Assert.IsNotNull(res);
            Assert.IsNotEmpty(res.TestEngine.Name);
            Assert.IsNotEmpty(res.TestEngine.Version);
            Assert.IsNotNull(res.TestEnvironment);
            Assert.IsNotNull(res.TestEnvironment.OrientationAngle);
            Assert.IsNotEmpty(res.TestEnvironment.OrientationType);
            Assert.IsNotEmpty(res.TestEnvironment.UserAgent);
            Assert.IsNotNull(res.TestEnvironment.WindowHeight);
            Assert.IsNotNull(res.TestEnvironment.WindowWidth);
            Assert.IsNotNull(res.TestRunner);
            Assert.IsNotEmpty(res.TestRunner.Name);
            Assert.IsNotNull(res.ToolOptions);
        }
    }
}
