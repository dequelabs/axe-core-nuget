using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeSelectorJsonConverterTest
    {
        #region Read tests
        [Test]
        public void CanReadSimpleSelector()
        {
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
            result.Should().Be(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "parent", "child" } }));
        }

        [Test]
        public void CanReadComplexShadowSelector()
        {
            var result = JsonConvert.DeserializeObject<AxeSelector>("[[\"parent-shadow-root\", \"parent-iframe-in-shadow\"], \"middle-without-shadow\", [\"child-shadow-root\", \"child-in-shadow\"]]");
            result.Should().Be(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                 new List<string> { "parent-shadow-root", "parent-iframe-in-shadow" },
                 new List<string> { "middle-without-shadow" },
                 new List<string> { "child-shadow-root", "child-in-shadow" },
            }));
        }

        [Test]
        [TestCase("null")]
        [TestCase("0")]
        [TestCase("{}")]
        [TestCase("[]")]
        [TestCase("[[]]")]
        [TestCase("[[[]]]")]
        [TestCase("[null]")]
        [TestCase("[0]")]
        [TestCase("[{}]")]
        [TestCase("[\"selector\", null]")]
        [TestCase("[\"selector\", [0]]")]
        [TestCase("[\"selector\", [{}]]")]
        [TestCase("[\"selector\", []]")]
        [TestCase("[\"selector\", [\"selector\", []]]")]
        public void ReadThrowsJsonSerializationExceptionForMalformedInput(string malformedInput)
        {
            Assert.That(() => JsonConvert.DeserializeObject<AxeSelector>(malformedInput), Throws.InstanceOf(typeof(JsonSerializationException)));
        }
        #endregion

        #region Write tests
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
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "parent", "child" } }));
            result.Should().Be("[[\"parent\",\"child\"]]");
        }

        [Test]
        public void CanWriteComplexShadowSelector()
        {
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                 new List<string> { "parent-shadow-root", "parent-iframe-in-shadow" },
                 new List<string> { "middle-without-shadow" },
                 new List<string> { "child-shadow-root", "child-in-shadow" },
            }));
            result.Should().Be("[[\"parent-shadow-root\",\"parent-iframe-in-shadow\"],\"middle-without-shadow\",[\"child-shadow-root\",\"child-in-shadow\"]]");
        }
        #endregion

        #region Array selector mode write tests
        [Test]
        public void WritesSimpleSelectorAsArrayWhenArraySelectorsEnabled()
        {
            var settings = AxeJsonSerializerSettings.WithFormatting(Formatting.None, arraySelectors: true);
            var result = JsonConvert.SerializeObject(new AxeSelector("simple string selector"), settings);
            result.Should().Be("[\"simple string selector\"]");
        }

        [Test]
        public void WritesIframeSelectorUnchangedWhenArraySelectorsEnabled()
        {
            var settings = AxeJsonSerializerSettings.WithFormatting(Formatting.None, arraySelectors: true);
            var result = JsonConvert.SerializeObject(new AxeSelector("child", new List<string> { "parent" }), settings);
            result.Should().Be("[\"parent\",\"child\"]");
        }

        [Test]
        public void WritesSimpleShadowSelectorUnchangedWhenArraySelectorsEnabled()
        {
            var settings = AxeJsonSerializerSettings.WithFormatting(Formatting.None, arraySelectors: true);
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "parent", "child" } }), settings);
            result.Should().Be("[[\"parent\",\"child\"]]");
        }

        [Test]
        public void WritesComplexShadowSelectorUnchangedWhenArraySelectorsEnabled()
        {
            var settings = AxeJsonSerializerSettings.WithFormatting(Formatting.None, arraySelectors: true);
            var result = JsonConvert.SerializeObject(AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                 new List<string> { "parent-shadow-root", "parent-iframe-in-shadow" },
                 new List<string> { "middle-without-shadow" },
                 new List<string> { "child-shadow-root", "child-in-shadow" },
            }), settings);
            result.Should().Be("[[\"parent-shadow-root\",\"parent-iframe-in-shadow\"],\"middle-without-shadow\",[\"child-shadow-root\",\"child-in-shadow\"]]");
        }

        [Test]
        public void ArraySelectorOutputRoundTripsBackToTheSameSelector()
        {
            var settings = AxeJsonSerializerSettings.WithFormatting(Formatting.None, arraySelectors: true);
            var original = new AxeSelector("simple string selector");

            var json = JsonConvert.SerializeObject(original, settings);

            JsonConvert.DeserializeObject<AxeSelector>(json).Should().Be(original);
        }
        #endregion
    }
}
