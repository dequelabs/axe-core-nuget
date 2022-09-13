using System;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using NUnit.Framework;

namespace Deque.AxeCore.Selenium.Test.RunPartial
{
    public class AnalyzeTests : TestBase
    {
        [Test]
        public void ShouldReturnResults()
        {
            GoToFixture("index.html");

            var res = new AxeBuilder(driver).Analyze();
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Inapplicable);
            Assert.IsNotNull(res.Incomplete);
            Assert.IsNotNull(res.Passes);
            Assert.IsNotNull(res.Violations);
        }

        [Test]
        public void ShouldThrowIfTopFrameErrors()
        {
            GoToFixture("crash.html");

            var axe = new AxeBuilder(driver, CustomSource($"{axeSource}{Environment.NewLine}{axeCrashScript}"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        public void ShouldThrowWhenInjectingProblematicSource()
        {
            GoToFixture("crash.html");

            var axe = new AxeBuilder(driver, CustomSource($"{axeSource}{Environment.NewLine}; throw new Error('Boom!')"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        public void ShouldThrowWhenSetupFails()
        {
            GoToFixture("crash.html");

            var axe = new AxeBuilder(driver, CustomSource($"{axeSource}{Environment.NewLine}; window.axe.utils = {{}}"));
            Assert.Throws<OpenQA.Selenium.JavaScriptException>(() => axe.Analyze());
        }

        [Test]
        public void ShouldReturnCorrectMetadata()
        {
            GoToFixture("index.html");

            var res = new AxeBuilder(driver)
                .Analyze();

            Assert.IsNotNull(res);
            Assert.IsNotEmpty(res.TestEngineName);
            Assert.IsNotEmpty(res.TestEngineVersion);
            Assert.IsNotNull(res.TestEnvironment);
            Assert.IsNotNull(res.TestEnvironment.OrientationAngle);
            Assert.IsNotEmpty(res.TestEnvironment.OrientationType);
            Assert.IsNotEmpty(res.TestEnvironment.UserAgent);
            Assert.IsNotNull(res.TestEnvironment.WindowHeight);
            Assert.IsNotNull(res.TestEnvironment.WindowWidth);
            Assert.IsNotNull(res.TestRunner);
            Assert.IsNotEmpty(res.TestRunner.Name);
            Assert.IsNotNull(res.ToolOptions);
            // todo: strongly type ToolOptions to test `reporter`
        }
    }
}
