using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeSelectorTest
    {
        #region IEquatable Tests
        [Test]
        public void LogicallyEquivalentSimpleSelectorsShouldBeEqual()
        {
            var simpleSelector1 = new AxeSelector("#some-id");
            var simpleSelector2 = new AxeSelector("#some-id");
            var simpleSelectorFromFrameShadowSelectors = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> { new List<string> { "#some-id" } });

            simpleSelector1.Should().Be(simpleSelector1);
            simpleSelector1.GetHashCode().Should().Be(simpleSelector1.GetHashCode());

            simpleSelector1.Should().Be(simpleSelector2);
            simpleSelector2.Should().Be(simpleSelector1);
            simpleSelector1.GetHashCode().Should().Be(simpleSelector2.GetHashCode());

            simpleSelector1.Should().Be(simpleSelectorFromFrameShadowSelectors);
            simpleSelectorFromFrameShadowSelectors.Should().Be(simpleSelector1);
            simpleSelector1.GetHashCode().Should().Be(simpleSelectorFromFrameShadowSelectors.GetHashCode());
        }

        [Test]
        public void LogicallyDifferentSimpleSelectorsShouldBeUnequal()
        {
            var simpleSelector1 = new AxeSelector("#first-id");
            var simpleSelector2 = new AxeSelector("#second-id");
            var simpleSelector3 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> { new List<string> { "#third-id" } });

            simpleSelector1.Should().NotBe(simpleSelector2);
            simpleSelector1.GetHashCode().Should().NotBe(simpleSelector2.GetHashCode());

            simpleSelector1.Should().NotBe(simpleSelector3);
            simpleSelector1.GetHashCode().Should().NotBe(simpleSelector3.GetHashCode());

            simpleSelector2.Should().NotBe(simpleSelector3);
            simpleSelector2.Should().NotBe(simpleSelector3.GetHashCode());
        }

        [Test]
        public void LogicallyEquivalentIframeSelectorsShouldBeEqual()
        {
            var iframeSelector1 = new AxeSelector("#some-id", new List<string> { "#frame1", "#frame2" });
            var iframeSelector2 = new AxeSelector("#some-id", new List<string> { "#frame1", "#frame2" });
            var iframeSelectorFromFrameShadowSelectors = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1" },
                new List<string> { "#frame2" },
                new List<string> { "#some-id" }
            });

            iframeSelector1.Should().Be(iframeSelector1);
            iframeSelector1.GetHashCode().Should().Be(iframeSelector1.GetHashCode());

            iframeSelector1.Should().Be(iframeSelector2);
            iframeSelector2.Should().Be(iframeSelector1);
            iframeSelector1.GetHashCode().Should().Be(iframeSelector2.GetHashCode());

            iframeSelector1.Should().Be(iframeSelectorFromFrameShadowSelectors);
            iframeSelectorFromFrameShadowSelectors.Should().Be(iframeSelector1);
            iframeSelector1.GetHashCode().Should().Be(iframeSelectorFromFrameShadowSelectors.GetHashCode());
        }

        [Test]
        public void LogicallyDifferentIframeSelectorsShouldBeUnequal()
        {
            var iframeSelector1 = new AxeSelector("#some-id", new List<string> { "#frame1", "#frame2" });
            var iframeSelector2 = new AxeSelector("#other-id", new List<string> { "#frame1", "#frame2" });
            var iframeSelector3 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1" },
                new List<string> { "#frame3" },
                new List<string> { "#some-id" }
            });

            iframeSelector1.Should().NotBe(iframeSelector2);
            iframeSelector1.GetHashCode().Should().NotBe(iframeSelector2.GetHashCode());

            iframeSelector1.Should().NotBe(iframeSelector3);
            iframeSelector1.GetHashCode().Should().NotBe(iframeSelector3.GetHashCode());

            iframeSelector2.Should().NotBe(iframeSelector3);
            iframeSelector2.Should().NotBe(iframeSelector3.GetHashCode());
        }

        [Test]
        public void LogicallyEquivalentFrameShadowSelectorsShouldBeEqual()
        {
            var complexSelector1 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1", "#shadow1", "#shadow2" },
                new List<string> { "#frame2" },
                new List<string> { "#shadow3", "#some-id" }
            });
            var complexSelector2 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1", "#shadow1", "#shadow2" },
                new List<string> { "#frame2" },
                new List<string> { "#shadow3", "#some-id" }
            });

            complexSelector1.Should().Be(complexSelector1);
            complexSelector1.GetHashCode().Should().Be(complexSelector1.GetHashCode());

            complexSelector1.Should().Be(complexSelector2);
            complexSelector2.Should().Be(complexSelector1);
            complexSelector1.GetHashCode().Should().Be(complexSelector2.GetHashCode());
        }

        [Test]
        public void LogicallyDifferentFrameShadowSelectorsShouldBeUnequal()
        {
            var complexSelector1 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1", "#shadow1", "#shadow2" },
                new List<string> { "#frame2" },
                new List<string> { "#shadow3", "#some-id" }
            });
            var complexSelector2 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1", "#shadow1", "#shadow2" },
                new List<string> { "#frame2" },
                new List<string> { "#shadow3", "#other-id" }
            });
            var complexSelector3 = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame1", "#shadow-not-1", "#shadow2" },
                new List<string> { "#frame2" },
                new List<string> { "#shadow3", "#some-id" }
            });

            complexSelector1.Should().NotBe(complexSelector2);
            complexSelector1.GetHashCode().Should().NotBe(complexSelector2.GetHashCode());

            complexSelector1.Should().NotBe(complexSelector3);
            complexSelector1.GetHashCode().Should().NotBe(complexSelector3.GetHashCode());

            complexSelector2.Should().NotBe(complexSelector3);
            complexSelector2.Should().NotBe(complexSelector3.GetHashCode());
        }

        [Test]
        public void SelectorsOfDifferentComplexityShouldBeUnequal()
        {
            var simpleSelector = new AxeSelector("#some-id");
            var iframeSelector = new AxeSelector("#some-id", new List<string> { "#frame" });
            var complexSelector = AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#frame" },
                new List<string> { "#shadow-root", "#some-id" }
            });

            simpleSelector.Should().NotBe(iframeSelector);
            simpleSelector.GetHashCode().Should().NotBe(iframeSelector.GetHashCode());

            simpleSelector.Should().NotBe(complexSelector);
            simpleSelector.GetHashCode().Should().NotBe(complexSelector.GetHashCode());

            iframeSelector.Should().NotBe(complexSelector);
            iframeSelector.Should().NotBe(complexSelector.GetHashCode());
        }
        #endregion

        #region Class doc comment examples
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

            selectorInIframe.FrameShadowSelectors.Should().HaveCount(2);
            selectorInIframe.FrameShadowSelectors[0].Should().ContainInOrder("#parent-iframe-element");
            selectorInIframe.FrameShadowSelectors[1].Should().ContainInOrder("#child-element");

            selectorInIframe.ToString().Should().Be("[\"#parent-iframe-element\", \"#child-element\"]");
        }

        [Test]
        public void ShadowSelectorBehaviorMatchesClassDocComment()
        {
            var selectorInShadowDomInIframe = AxeSelector.FromFrameShadowSelectors(new List<IList<string>>
            {
                new List<string> { "#parent-iframe-element" },
                new List<string> { "#shadow-root-in-iframe", "#child-in-shadow-root" }
            });

            Assert.That(() => selectorInShadowDomInIframe.Selector, Throws.InvalidOperationException);

            Assert.That(() => selectorInShadowDomInIframe.FrameSelectors, Throws.InvalidOperationException);

            selectorInShadowDomInIframe.FrameShadowSelectors.Should().HaveCount(2);
            selectorInShadowDomInIframe.FrameShadowSelectors[0].Should().ContainInOrder("#parent-iframe-element");
            selectorInShadowDomInIframe.FrameShadowSelectors[1].Should().ContainInOrder("#shadow-root-in-iframe", "#child-in-shadow-root");

            selectorInShadowDomInIframe.ToString().Should().Be("[\"#parent-iframe-element\", [\"#shadow-root-in-iframe\", \"#child-in-shadow-root\"]]");
        }
        #endregion

        #region Constructor error handling
        [Test]
        public void SimpleConstructorShouldThrowForInvalidInput()
        {
            Assert.That(() => new AxeSelector(null), Throws.ArgumentNullException);
        }

        [Test]
        public void FrameConstructorShouldThrowForInvalidInput()
        {
            Assert.That(() => new AxeSelector(null, new List<string> { "valid frame selector" }), Throws.ArgumentNullException);
            Assert.That(() => new AxeSelector("valid selector", null), Throws.ArgumentNullException);
            Assert.That(() => new AxeSelector("valid selector", new List<string>()), Throws.ArgumentException);
        }

        [Test]
        public void ShadowFrameSelectorsFactoryMethodShouldThrowForInvalidInput()
        {
            Assert.That(() => AxeSelector.FromFrameShadowSelectors(null), Throws.ArgumentNullException);
            Assert.That(() => AxeSelector.FromFrameShadowSelectors(new List<IList<string>>()), Throws.ArgumentException);
            Assert.That(() => AxeSelector.FromFrameShadowSelectors(new List<IList<string>> { new List<string>() }), Throws.ArgumentException);
            Assert.That(() => AxeSelector.FromFrameShadowSelectors(new List<IList<string>> {
                new List<string> { "#valid-frame" },
                new List<string>(), // empty, invalid
                new List<string> { "#valid-child" },
            }), Throws.ArgumentException);
        }
        #endregion
    }
}
