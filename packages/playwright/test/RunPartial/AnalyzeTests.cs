using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{
    public class AnalyzeTests : TestBase
    {
        [Test]
        public async Task ShouldReturnResults()
        {
            await GoToFixture("index.html");

            var res = await Page!.RunAxe();
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Inapplicable);
            Assert.IsNotNull(res.Incomplete);
            Assert.IsNotNull(res.Passes);
            Assert.IsNotNull(res.Violations);
        }

        [Test]
        public async Task ShouldThrowIfTopFrameErrors()
        {
            await GoToFixture("crash.html");

            Assert.ThrowsAsync<PlaywrightException>(() => RunWithCustomAxe(CustomSource($"{axeSource}{Environment.NewLine}{axeCrashScript}")));
        }

        [Test]
        public async Task ShouldThrowWhenInjectingProblematicSource()
        {
            await GoToFixture("crash.html");

            Assert.ThrowsAsync<PlaywrightException>(async () => await RunWithCustomAxe(CustomSource($"{axeSource}{Environment.NewLine}; throw new Error('Boom!')")));
        }

        [Test]
        public async Task ShouldThrowWhenSetupFails()
        {
            await GoToFixture("crash.html");

            Assert.ThrowsAsync<PlaywrightException>(async () => await RunWithCustomAxe(CustomSource($"{axeSource}{Environment.NewLine}; window.axe.utils = {{}}")));
        }

        [Test]
        public async Task ShouldReturnCorrectMetadata()
        {
            await GoToFixture("index.html");

            var res = await Page!.RunAxe();

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
        }
    }
}
