using NUnit.Framework;
using Deque.AxeCore.Commons;
using Moq;
using FluentAssertions;
using System;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class BundledAxeScriptProviderTest
    {
        // private static BundledAxeScriptProvider testProvider = new BundledAxeScriptProvider();
        [Test]
        public void GetScriptCalled()
        {
            var scriptProvider = new BundledAxeScriptProvider();
            Assert.DoesNotThrow(() =>
            {
                var readResult = scriptProvider.GetScript();
                readResult.Should().NotBeNull();
            });
        }
    }
}
