using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeRunContextTest
    {
        [Test]
        public void ShouldSerializeObject()
        {
            var context = new AxeRunContext()
            {
                Include = new List<AxeSelector> { new AxeSelector("#included") },
                Exclude = new List<AxeSelector> { new AxeSelector("#excluded") },
            };

            var expectedContent = "{\"include\":[\"#included\"],\"exclude\":[\"#excluded\"]}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }

        [Test]
        public void ShouldNotIncludeNullPropertiesOnSerializing()
        {
            var context = new AxeRunContext();

            var expectedContent = "{}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }
    }
}
