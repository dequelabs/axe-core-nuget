using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeSelectorJsonConverterTest
    {
        [Test]
        public void CanReadSimpleSelector() {
            var result = JsonConvert.DeserializeObject<AxeSelector>("\"simple string selector\"");
            result.Should().Be(new AxeSelector("simple string selector"));
        }

        [Test]
        public void CanReadIframeSelector()
        {
            var result = JsonConvert.DeserializeObject<AxeSelector>("[\"parent\", \"child\"]");
            result.Should().Be(new AxeSelector("child", new List<string> { "parent" }));
        }

        [Test]
        public void CanReadSimpleShadowSelector()
        {
            var result = JsonConvert.DeserializeObject<AxeSelector>("[[\"parent\", \"child\"]]");
            result.Should().Be(AxeSelector.FromFrameShadowSelectors(new List<List<string>> {
                new List<string> { "parent", "child" } }));
        }

        [Test]
        public void CanReadComplexShadowSelector()
        {
            var result = JsonConvert.DeserializeObject<AxeSelector>("[[\"parent-shadow-root\", \"parent-iframe-in-shadow\"], \"middle-without-shadow\", [\"child-shadow-root\", \"child-in-shadow\"]]");
            result.Should().Be(AxeSelector.FromFrameShadowSelectors(new List<List<string>> {
                 new List<string> { "parent-shadow-root", "parent-iframe-in-shadow" },
                 new List<string> { "middle-without-shadow" },
                 new List<string> { "child-shadow-root", "child-in-shadow" },
            }));
        }

        [Test]
        public void CanWriteSimpleSelector()
        {
            var result = JsonConvert.SerializeObject(new AxeSelector("simple string selector"));
            result.Should().Be("\"simple string selector\"");
        }

        [Test]
        public void CanWriteIframeSelector()
        {
            var result = JsonConvert.SerializeObject(new AxeSelector("child", new List<string> { "parent" }));
            result.Should().Be("[\"parent\",\"child\"]");
        }

        [Test]
        public void CanWriteSimpleShadowSelector()
        {
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<List<string>> {
                new List<string> { "parent", "child" } }));
            result.Should().Be("[[\"parent\",\"child\"]]");
        }

        [Test]
        public void CanWriteComplexShadowSelector()
        {
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<List<string>> {
                 new List<string> { "parent-shadow-root", "parent-iframe-in-shadow" },
                 new List<string> { "middle-without-shadow" },
                 new List<string> { "child-shadow-root", "child-in-shadow" },
            }));
            result.Should().Be("[[\"parent-shadow-root\",\"parent-iframe-in-shadow\"],\"middle-without-shadow\",[\"child-shadow-root\",\"child-in-shadow\"]]");
        }

        // TODO: test error paths
    }
}
