using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{

    [Category("RP")]
    public class UseLegacyTests : TestBase
    {
        [Test]
        public async Task ShouldRunLegacyAxe()
        {
            await GoToFixture("index.html");

#pragma warning disable CS0618
            var results = await RunLegacyWithCustomAxe(axeCoreLegacy);
#pragma warning restore CS0618

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Passes.Any());
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
