using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeSelectorTest
    {
        [Test]
        public void SimpleSelectorBehaviorMatchesClassDocComment()
        {
            var simpleSelector = new AxeSelector("#some-element-id");

            simpleSelector.Selector.Should().Be("#some-element-id");
            simpleSelector.FrameSelectors.Should().ContainSingle()
                .Which.Should().Be("#some-element-id");
            simpleSelector.FrameShadowSelectors.Should().ContainSingle()
                .Which.Should().ContainSingle()
                .Which.Should().Be("#some-element-id");
            simpleSelector.ToString().Should().Be("#some-element-id");
        }

        [Test]
        public void IframeSelectorBehaviorMatchesClassDocComment()
        {
            var selectorInIframe = new AxeSelector("#child-element", new List<string> { "#parent-iframe-element" });

            Assert.That(() => selectorInIframe.Selector, Throws.InvalidOperationException);
            selectorInIframe.FrameSelectors.Should().ContainInOrder("#parent-iframe-element", "#child-element");
            selectorInIframe.FrameShadowSelectors.Should().ContainInOrder(
                new List<string> { "#parent-iframe-element" },
                new List<string> { "#child-element" });
            selectorInIframe.ToString().Should().Be("[\"#parent-iframe-element\", \"#child-element\"]");
        }

        [Test]
        public void ShadowSelectorBehaviorMatchesClassDocComment()
        {
            var selectorInShadowDomInIframe = AxeSelector.FromFrameShadowSelectors(new List<List<string>>
            {
                new List<string> { "#parent-iframe-element" },
                new List<string> { "#shadow-root-in-iframe", "#child-in-shadow-root" }
            });

            Assert.That(() => selectorInShadowDomInIframe.Selector, Throws.InvalidOperationException);
            Assert.That(() => selectorInShadowDomInIframe.FrameSelectors, Throws.InvalidOperationException);
            selectorInShadowDomInIframe.FrameShadowSelectors.Should().ContainInOrder(
                new List<string> { "#parent-iframe-element" },
                new List<string> { "#shadow-root-in-iframe", "#child-in-shadow-root" });
            selectorInShadowDomInIframe.ToString().Should().Be("[\"#parent-iframe-element\", [\"#shadow-root-in-iframe\", \"#child-element\"]]");
        }
    }
}
