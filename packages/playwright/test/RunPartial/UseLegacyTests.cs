using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{

    public class UseLegacyTests : TestBase
    {
        [Test]
        public async Task ShouldRunLegacyAxe()
        {
            await GoToFixture("index.html");

#pragma warning disable CS0618
            var results = await RunWithCustomAxe(axeCoreLegacy);
#pragma warning restore CS0618

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
        }

        [Test]
        public async Task ShouldPreventCrossOriginFrameTesting()
        {
            await GoToFixture("cross-origin.html");

#pragma warning disable CS0618
            var results = await Page!.RunAxeLegacy(null);
#pragma warning restore CS0618

            foreach (var rule in results.Incomplete)
            {
                if (rule.Id == "frame-tested")
                {
                    return;
                }
            }
            Assert.Fail("Could not find frame-tested");
        }

        [Test]
        public async Task LegacySourceShouldThrowIfTopFrameErrors()
        {
            await GoToFixture("crash.html");

            Assert.ThrowsAsync<PlaywrightException>(() => RunWithCustomAxe(CustomSource($"{axeCoreLegacy}{Environment.NewLine}{axeCrashScript}")));
        }

        [Test]
        public async Task LegacySourceShouldTestCrossOrigin()
        {
            await GoToFixture("cross-origin.html");

            var res = await RunWithCustomAxe(CustomSource(axeCoreLegacy));
            foreach (var rule in res.Incomplete)
            {
                if (rule.Id == "frame-tested")
                {
                    Assert.Fail("Cross origin frames should be tested");
                }
            }
        }

        [Test]
        public async Task ShouldNotSetAOWhenRunPartialAndNotLegacy()
        {
            await GoToFixture("index.html");
            await Page!.RunAxe();

            var allowedOrigins = await AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins()));
        }

        [Test]
        public async Task ShouldNotSetAOWhenRunPartialAndLegacy()
        {
            await GoToFixture("index.html");
#pragma warning disable CS0618
            await Page!.RunAxeLegacy(null);
#pragma warning restore CS0618
            var allowedOrigins = await AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins()));
        }

        [Test]
        public async Task ShouldNotSetAOWhenLegacySourceAndLegacyMode()
        {
            await GoToFixture("index.html");
#pragma warning disable CS0618
            await RunLegacyWithCustomAxe(CustomSource($"{{ {axeSource}; {axeForceLegacy}; }}"));
#pragma warning restore CS0618
            var allowedOrigins = await AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo(ExpectedAllowedOrigins()));
        }

        [Test]
        public async Task ShouldSetAOWhenLegacySourceAndNotLegacyMode()
        {

            await GoToFixture("index.html");
            await RunWithCustomAxe(CustomSource($"{{ {axeSource}; {axeForceLegacy}; }}"));

            var allowedOrigins = await AllowedOrigins();
            Assert.That(allowedOrigins, Is.EqualTo("*"));
        }

        private async Task<string> AllowedOrigins()
        {
            return await Page!.EvaluateAsync<string>("() => axe._audit.allowedOrigins[0]");
        }

        private string ExpectedAllowedOrigins()
        {
            return "http://127.0.0.1:3000";
        }

    }
}
