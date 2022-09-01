using NUnit.Framework;
using Deque.AxeCore.Commons;
using FluentAssertions;
using System;

namespace Deque.AxeCore.Commons.Test 
{
    [TestFixture]
    [NonParallelizable] 
    public class EmbeddedResourceProviderTest 
    {
        private static readonly string sampleFileName = "sampleFile.txt";
        [Test]
        public void PassedValidString() {
            var readResult = EmbeddedResourceProvider.ReadEmbeddedFile(sampleFileName);
            readResult.Should().Be("sample output");
        }
    }
}