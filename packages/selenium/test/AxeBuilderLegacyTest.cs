using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Selenium.Test
{
    public class LegacyTestAxeScriptProvider : IAxeScriptProvider
    {
        public static string stubAxeScript = "stub axe script";
        public string GetScript()
        {
            return stubAxeScript;
        }
    }

    [TestFixture]
    [NonParallelizable]
    public class LegacyAxeBuilderTest
    {
        private static Mock<IWebDriver> webDriverMock = new Mock<IWebDriver>();
        private static Mock<IJavaScriptExecutor> jsExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
        private static Mock<ITargetLocator> targetLocatorMock = new Mock<ITargetLocator>();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly AxeBuilderOptions stubAxeBuilderOptions = new AxeBuilderOptions
        {
            ScriptProvider = new LegacyTestAxeScriptProvider()
        };

        private readonly object testAxeResult = new
        {
            violations = new object[] { },
            passes = new object[] { },
            inapplicable = new object[] { },
            incomplete = new object[] { },
            timestamp = DateTimeOffset.Now,
            url = "www.test.com"
        };

        [Test]
        public void LegacyThrowWhenDriverIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                //arrange / act /assert
                var axeBuilder = new AxeBuilder(null, stubAxeBuilderOptions);
                axeBuilder.Should().NotBeNull();
            });
        }

        [Test]
        public void LegacyThrowWhenOptionsAreNull()
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
        public void LegacyShouldHandleIfOptionsAndContextNotSet()
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
        public void LegacyShouldPassContextIfIncludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { "#div1" } })
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
        public void LegacyShouldPassContextIfIncludeAndExcludeSet()
        {
            var includeSelector = "#div1";
            var excludeSelector = "#div2";
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { includeSelector } }),
                Exclude = new List<string[]>(new string[][] { new string[] { excludeSelector } })
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
        public void LegacyShouldPassContextIfExcludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Exclude = new List<string[]>(new string[][] { new string[] { "#div1" } })
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
        public void LegacyShouldPassRunOptionsIfDeprecatedOptionsSet()
        {
            var expectedOptions = "deprecated run options";

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);
#pragma warning disable CS0618
            builder.Options = expectedOptions;
#pragma warning restore CS0618

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void LegacyShouldPassRunOptionsIfDeprecatedOptionsSetWithContextElement()
        {
            var expectedOptions = "deprecated run options";
            var expectedContext = new Mock<IWebElement>();

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanElementCall(expectedContext.Object, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);
#pragma warning disable CS0618
            builder.Options = expectedOptions;
#pragma warning restore CS0618

            var result = builder.Analyze(expectedContext.Object);

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void LegacyShouldPassRuleConfig()
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
        public void LegacyShouldPassRunOptionsWithTagConfig()
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
        public void LegacyShouldPassRunOptions()
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
        public void LegacyShouldThrowIfNullParameterPassed()
        {
            SetupVerifiableAxeInjectionCall();

            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(webDriverMock.Object, null));
            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(null));

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);

            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.DisableRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithTags(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Include(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Exclude(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithOptions(null));
        }

        [Test]
        public void LegacyShouldThrowIfEmptyParameterPassed()
        {
            var values = new string[] { "val1", "" };

            SetupVerifiableAxeInjectionCall();

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);

            VerifyExceptionThrown<ArgumentException>(() => builder.WithRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.DisableRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.WithTags(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.Include(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.Exclude(values));
        }

        [Test]
        public void LegacyShouldThrowIfDeprecatedOptionsIsUsedWithNewOptionsApis()
        {
            SetupVerifiableAxeInjectionCall();

            var builder = new AxeBuilder(webDriverMock.Object, stubAxeBuilderOptions);
#pragma warning disable CS0618
            builder.Options = "{xpath:true}";
#pragma warning restore CS0618

            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithRules("rule-1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.DisableRules("rule-1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithTags("tag1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithOptions(new AxeRunOptions() { Iframes = true }));
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
            // Pretend we do not support axe.runPartial so that old code path is used
            jsExecutorMock
                .Setup(js => js.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartialExists.js")))
                .Returns(false);
            jsExecutorMock
                .Setup(js => js.ExecuteScript(LegacyTestAxeScriptProvider.stubAxeScript)).Verifiable();
        }

        private void SetupVerifiableScanCall(string serializedContext, string serialzedOptions)
        {
            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                EmbeddedResourceProvider.ReadEmbeddedFile("legacyScan.js"),
                It.Is<string>(context => context == serializedContext),
                It.Is<string>(options => options == serialzedOptions))).Returns(testAxeResult).Verifiable();
        }

        private void SetupVerifiableScanElementCall(IWebElement elementContext, string serialzedOptions)
        {
            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                EmbeddedResourceProvider.ReadEmbeddedFile("legacyScan.js"),
                elementContext,
                It.Is<string>(options => options == serialzedOptions))).Returns(testAxeResult).Verifiable();
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

    }
}
