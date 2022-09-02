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
        //Tests for creating a FileAxeScriptProvider
        // [Test]
        // public void ConstructorPassedValidFile()
        // {
        //     //Construction
        //     var scriptProvider = new FileAxeScriptProvider($"Deque.AxeCore.Commons.Resources.sampleFile.txt");
        //     scriptProvider.Should().NotBeNull();
        //     // scriptProvider._filePath.Should().Be(testFile);

        //     //Checking GetScript
        //     var readResult = scriptProvider.GetScript();
        //     readResult.Should().Be("sample output");
        // }

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
                var scriptProvider = new FileAxeScriptProvider("");                
                scriptProvider.Should().NotBeNull();
            });
        }

        [Test]
        public void ConstructorPassedNonexistentFile()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var scriptProvider = new FileAxeScriptProvider("foo.html");
                scriptProvider.Should().NotBeNull();
            });
        }

        [Test]
        //Add a file to the directory
        //Create a FileAxeScriptProvider
        //Remove the file from the directory
        //Call GetScript and Assert the InvalidOperationException is thrown
            // Assert.Throws<InvalidOperationException>(() =>
            // {
            //     scriptProvider.GetScript();
            //     scriptProvider.Should().NotBeNull();
            // });    
    }
}