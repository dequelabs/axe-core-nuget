using NUnit.Framework;
using Deque.AxeCore.Commons;
using Moq;
using FluentAssertions;
using System;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    [NonParallelizable]
    public class BundledAxeScriptProviderTest
    {
        // private static Mock<EmbeddedResourceProvider> mock = new Mock<EmbeddedResourceProvider>();
        private static BundledAxeScriptProvider testProvider = new BundledAxeScriptProvider();
        [Test]
        public void GetScriptCalled() 
        {
            Assert.DoesNotThrow(() =>
            {
                var readResult = testProvider.GetScript();
                readResult.Should().NotBeNull();
            });
            // var getScriptCalled = readResult != null ? readResult : null;
            // Console.WriteLine(getScriptCalled);
            // getScriptCalled.Should().NotBeNull();
            // mock.Setup(mock => mock.GetScript())
            //     .Returns("foo string")
            //     .Verifiable();
            // var testBundledProvider = new BundledAxeScriptProvider();
            // var scriptResult = testBundledProvider.GetScript();
            // mock.Verify(mock => mock.GetScript(), Times.Once);
        }
    }
}