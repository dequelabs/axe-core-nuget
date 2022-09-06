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
        private static BundledAxeScriptProvider testProvider = new BundledAxeScriptProvider();
        [Test]
        public void GetScriptCalled() 
        {
            Assert.DoesNotThrow(() =>
            {
                var readResult = testProvider.GetScript();
                readResult.Should().NotBeNull();
            });
        }
    }
}