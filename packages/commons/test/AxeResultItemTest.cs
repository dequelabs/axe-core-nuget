using FluentAssertions;
using NUnit.Framework;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeResultItemTest
    {
        [Test]
        public void ToStringShouldIncludeEnoughInfoToBeActionable()
        {
            var node = new AxeResultNode
            {
                All = new AxeResultCheck[] { },
                Any = new AxeResultCheck[] { },
                None = new AxeResultCheck[] { },
                Ancestry = null,
                Target = new AxeSelector("#selector"),
                Html = "stub-html",
                Impact = "stub-impact",
                XPath = null,
            };

            var item = new AxeResultItem
            {
                Description = "stub desc",
                Help = "stub help",
                HelpUrl = "http://example.com/help",
                Id = "stub-id",
                Impact = "stub-impact",
                Tags = new string[] { "tag-1", "tag-2" },
                Nodes = new AxeResultNode[] { node },
            };

            var toStringOutput = item.ToString();

            toStringOutput.Should().ContainAll(new string[] {
                item.Description,
                item.Help,
                item.HelpUrl,
                item.Id,
                item.Impact,
                item.Tags[0],
                item.Tags[1],
                node.Target.ToString(),
                node.Html,
                node.Impact,
            });
        }
    }
}
