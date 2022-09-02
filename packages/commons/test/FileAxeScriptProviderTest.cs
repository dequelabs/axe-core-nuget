using NUnit.Framework;
using Deque.AxeCore.Commons;
using Moq;
using FluentAssertions;
using System;
using System.IO;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    [NonParallelizable]
    public class FileAxeScriptProviderTest
    {
        private readonly static string testFile = Path.Join(Directory.GetParent(Environment.CurrentDirectory), "/src/Resources/sampleFile.txt");
        //Tests for creating a FileAxeScriptProvider
        [Test]
        public void ConstructorPassedValidFile()
        {
            //Construction
            // Console.WriteLine(Environment.CurrentDirectory);
            // Console.WriteLine(Directory.GetParent(Environment.CurrentDirectory));
            var scriptProvider = new FileAxeScriptProvider(testFile);
            scriptProvider.Should().NotBeNull();
            // scriptProvider._filePath.Should().Be(testFile);

            //Checking GetScript
            var readResult = scriptProvider.GetScript();
            readResult.Should().Be("sample output");
        }

        [Test]
        public void ConstructorPassedNullOrEmptyString()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider(null);                
                scriptProvider.Should().NotBeNull();
            });
            
            Assert.Throws<ArgumentNullException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider(" ");                
                scriptProvider.Should().NotBeNull();
            });
        }

        [Test]
        public void ConstructorPassedNonexistentFile()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider("foo.html");
                // scriptProvider.Should().NotBeNull();
            });
            //Assert an error was thrown
        }    
    }
}