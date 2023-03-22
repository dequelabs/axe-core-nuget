using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Deque.AxeCore.Commons;
using OpenQA.Selenium.DevTools.V102.Runtime;

namespace Deque.AxeCore.Selenium.Test
{
    public class TestAxeScriptProvider : IAxeScriptProvider
    {
        public static string stubAxeScript = "stub axe script";
        public string GetScript()
        {
            return stubAxeScript;
        }
    }

    [TestFixture]
    [NonParallelizable]
    public class AxeBuilderTest
    {
        private static Mock<IWebDriver> webDriverMock = new Mock<IWebDriver>();
        private static Mock<IJavaScriptExecutor> jsExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
        private static Mock<ITargetLocator> targetLocatorMock = new Mock<ITargetLocator>();
        private static Mock<INavigation> navigationMock = new Mock<INavigation>();
        private static Mock<IPostAnalyzeCallback> postAnalyzeCallbackMock = new Mock<IPostAnalyzeCallback>();

        private static readonly AxeBuilderOptions stubAxeBuilderOptions = new AxeBuilderOptions
        {
            ScriptProvider = new TestAxeScriptProvider()
        };

        private static readonly object testAxeResult = new
        {
            violations = new object[] { },
            passes = new object[] { },
            inapplicable = new object[] { },
            incomplete = new object[] { },
            timestamp = DateTimeOffset.Now,
            url = "www.test.com"
        };

        [Test]
        public void ThrowWhenDriverIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                //arrange / act /assert
                var axeBuilder = new AxeBuilder(null, stubAxeBuilderOptions);
                axeBuilder.Should().NotBeNull();
            });
        }

        [Test]
        public void ThrowWhenOptionsAreNull()
        {
            //arrange
            var driver = new Mock<IWebDriver>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                // act / assert
                var axeBuilder = new AxeBuilder(driver.Object, null);
                axeBuilder.Should().NotBeNull();
            });
        }

        [Test]
        public void ShouldHandleIfOptionsAndContextNotSet()
        {

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, "{}");

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);
            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();

        }

        [Test]
        public void ShouldPassContextIfIncludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<AxeSelector> { new AxeSelector("#div1") }
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions).Include("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassContextIfIncludeAndExcludeSet()
        {
            var includeSelector = "#div1";
            var excludeSelector = "#div2";
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<AxeSelector> { new AxeSelector(includeSelector) },
                Exclude = new List<AxeSelector> { new AxeSelector(excludeSelector) },
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions).Include(includeSelector).Exclude(excludeSelector);

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [Test]
        public void ShouldPassContextIfExcludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Exclude = new List<AxeSelector> { new AxeSelector("#div1") }
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions).Exclude("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassRuleConfig()
        {
            var expectedRules = new List<string> { "rule1", "rule2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "rule",
                    Values = expectedRules
                },
                Rules = new Dictionary<string, RuleOptions>()
                {
                    { "excludeRule1", new RuleOptions(){ Enabled = false} },
                    { "excludeRule2", new RuleOptions(){ Enabled = false } }
                }

            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions)
                .DisableRules("excludeRule1", "excludeRule2")
                .WithRules("rule1", "rule2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassRunOptionsWithTagConfig()
        {
            var expectedTags = new List<string> { "tag1", "tag2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = expectedTags
                },
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions)
                .WithTags("tag1", "tag2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassRunOptions()
        {
            var runOptions = new AxeRunOptions()
            {
                Iframes = true,
                Rules = new Dictionary<string, RuleOptions>() { { "rule1", new RuleOptions() { Enabled = false } } }
            };

            var expectedRunOptions = SerializeObject(runOptions);

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedRunOptions);

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions)
                .WithOptions(runOptions);

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldInvokePostAnalyzeHookWhenAdded()
        {
            SetupVerifiablePostAnalyzeCallback();
            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, "{}");

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions)
                .WithPostAnalyzeHook(postAnalyzeCallbackMock.Object.PostAnalyzeCallback);

            var result = builder.Analyze();

            postAnalyzeCallbackMock.VerifyAll();
        }

        [Test]
        public void ShouldThrowIfNullParameterPassed()
        {
            SetupVerifiableAxeInjectionCall();

            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(webDriverMock.Object, null));
            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(null));

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);

            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.DisableRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithTags(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Include((string)null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Include((AxeSelector)null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Exclude((string)null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Exclude((AxeSelector)null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithOptions(null));
        }

        [Test]
        public void ShouldThrowIfEmptyParameterPassed()
        {
            var values = new string[] { "val1", "" };

            SetupVerifiableAxeInjectionCall();

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);

            VerifyExceptionThrown<ArgumentException>(() => builder.WithRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.DisableRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.WithTags(values));
        }

        private void VerifyExceptionThrown<T>(Action action) where T : Exception
        {
            action.Should().Throw<T>();
        }

        private void VerifyAxeResult(AxeResult result)
        {
            result.Should().NotBeNull();
            result.Inapplicable.Should().NotBeNull();
            result.Incomplete.Should().NotBeNull();
            result.Passes.Should().NotBeNull();
            result.Violations.Should().NotBeNull();

            result.Inapplicable.Length.Should().Be(0);
            result.Incomplete.Length.Should().Be(0);
            result.Passes.Length.Should().Be(0);
            result.Violations.Length.Should().Be(0);
        }

        private static void SetupVerifiableAxeInjectionCall()
        {
            webDriverMock
                .Setup(d => d.WindowHandles)
                .Returns(new ReadOnlyCollection<string>(new List<string>() { "some window id" }));
            webDriverMock
                .Setup(d => d.Navigate())
                .Returns(navigationMock.Object);

            jsExecutorMock
                .Setup(js => js.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartialExists.js")))
                .Returns(true);
            jsExecutorMock
                .Setup(js => js.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("getFrameContexts.js"), It.IsAny<object>()))
                .Returns("[]");

            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                EmbeddedResourceProvider.ReadEmbeddedFile("finishRun.js"),
                It.IsAny<string>(),
                It.IsAny<string>())).Returns(testAxeResult);
            webDriverMock.Setup(d => d.SwitchTo()).Returns(targetLocatorMock.Object);
        }

        private void SetupVerifiableScanCall(string serializedContext, string serialzedOptions)
        {
            jsExecutorMock
                .Setup(js => js.ExecuteAsyncScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartial.js"),
                It.Is<string>(context => context == serializedContext),
                It.Is<string>(options => options == serialzedOptions))
                )
                .Returns("{}")
                .Verifiable();
        }

        private void SetupVerifiableScanElementCall(IWebElement elementContext, string serialzedOptions)
        {
            jsExecutorMock
                .Setup(js => js.ExecuteAsyncScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartial.js"),
                elementContext,
                It.Is<string>(options => options == serialzedOptions))
                )
                .Returns("{}")
                .Verifiable();
        }

        private void SetupVerifiablePostAnalyzeCallback()
        {
            postAnalyzeCallbackMock.Setup(
                m => m.PostAnalyzeCallback(It.IsAny<AxeResult>(),
                It.IsAny<AxeRunOptions>(),
                It.IsAny<IWebDriver>()))
                .Verifiable();
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, AxeJsonSerializerSettings.Default);
        }

        /// <summary>
        /// Interface to simplify callback verification in Unit Tests
        /// </summary>
        public interface IPostAnalyzeCallback
        {
            void PostAnalyzeCallback(AxeResult result, AxeRunOptions options, IWebDriver driver);
        }
    }
}
