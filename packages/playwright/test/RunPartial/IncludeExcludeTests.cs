using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Playwright.Test.RunPartial
{
    public class IncludeExcludeTests : TestBase
    {
        [Test]
        public async Task ShouldHonorInclude()
        {
            await GoToResource("context.html");

            var context = new AxeRunContext
            {
                Include = new List<AxeSelector> { new AxeSelector(".include") },
                Exclude = null
                            ,
            };
            var includeRes = await Page!.RunAxe(context);

            var normalRes = await Page!.RunAxe();
            Assert.That(includeRes.Violations.Length, Is.LessThan(normalRes.Violations.Length));
        }

        [Test]
        public async Task ShouldHonorExclude()
        {
            await GoToResource("context.html");

            var context = new AxeRunContext
            {
                Include = null,
                Exclude = new List<AxeSelector> { new AxeSelector(".exclude") }
                            ,
            };
            var excludeRes = await Page!.RunAxe(context);

            var normalRes = await Page!.RunAxe();
            Assert.That(excludeRes.Violations.Length, Is.LessThan(normalRes.Violations.Length));
        }
    }
}
