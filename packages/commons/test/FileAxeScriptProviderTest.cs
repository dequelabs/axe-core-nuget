using NUnit.Framework;
using Deque.AxeCore.Commons;
using Moq;
using FluentAssertions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    [NonParallelizable]
    public class FileAxeScriptProviderTest
    {
        private static readonly string testContext = TestContext.CurrentContext.TestDirectory.ToString();
        private readonly string basePath = testContext.Split(new string[] { "test" }, StringSplitOptions.None)[0];

        [Test]
        public void ConstructorPassedValidFile()
        {
            var absolutePath = Path.Combine(basePath, "test", "Resources", "sampleFile.txt");

            var scriptProvider = new FileAxeScriptProvider(absolutePath);
            scriptProvider.Should().NotBeNull();
            var readResult = scriptProvider.GetScript();
            readResult.Should().Be("sample output");
        }

        [Test]
        public void ConstructorPassedInvalidFilePathValues()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider(null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider("");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider("sample.html");
            });
        }

        [Test]
        public void GetScriptForNonexistentFile()
        {
            var testFilePath = Path.Combine(typeof(FileAxeScriptProviderTest).Assembly.Location, "..", "foo.txt");
            File.Create(testFilePath).Dispose();
            var scriptProvider = new FileAxeScriptProvider(testFilePath);
            File.Delete(testFilePath);
            Assert.Throws<InvalidOperationException>(() =>
            {
                scriptProvider.GetScript();
                scriptProvider.Should().NotBeNull();
            });
        }
    }
}
