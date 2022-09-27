using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeRunOptionsTest
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
        };

        [Test]
        public void ShouldSerializeRunOnlyOption()
        {
            var options = new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = new List<string> { "tag1", "tag2" }
                }
            };

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);
            var expectedObject = "{\"runOnly\":{\"type\":\"tag\",\"values\":[\"tag1\",\"tag2\"]}}";

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }

        [Test]
        public void ShouldSerializeRuleOptions()
        {
            var options = new AxeRunOptions()
            {
                Rules = new Dictionary<string, RuleOptions>
                {
                    {"enabledRule", new RuleOptions(){ Enabled = true} },
                    {"disabledRule", new RuleOptions(){ Enabled = false} },
                    {"rule3WithoutOptionsData", new RuleOptions(){ } },
                }
            };
            var expectedObject = "{\"rules\":{\"enabledRule\":{\"enabled\":true},\"disabledRule\":{\"enabled\":false},\"rule3WithoutOptionsData\":{}}}";

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }

        [Test]
        public void ShouldSerializeLiteralTypes()
        {
            var options = new AxeRunOptions()
            {
                Selectors = true,
                Ancestry = true,
                AbsolutePaths = true,
                FrameWaitTime = 10,
                Iframes = true,
                RestoreScroll = true,
                PingWaitTime = 100,
            };
            var expectedObject = "{\"selectors\":true,\"ancestry\":true,\"absolutePaths\":true,\"iframes\":true,\"restoreScroll\":true,\"frameWaitTime\":10,\"pingWaitTime\":100}";

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }

        [Test]
        public void ShouldSerializeResultTypes()
        {
            var options = new AxeRunOptions()
            {
                ResultTypes = new HashSet<ResultType>() { ResultType.Inapplicable, ResultType.Incomplete, ResultType.Passes, ResultType.Violations }
            };

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            var expectedObject = "{\"resultTypes\":[\"inapplicable\",\"incomplete\",\"passes\",\"violations\"]}";

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }
    }
}
