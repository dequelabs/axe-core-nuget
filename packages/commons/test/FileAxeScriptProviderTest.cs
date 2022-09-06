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
        //Tests for creating a FileAxeScriptProvider
        [Test]
        public void ConstructorPassedValidFile()
        {
            var targetFramework = RuntimeInformation.FrameworkDescription.ToString();
            var basePath = targetFramework.Contains("Framework") ? Directory.GetParent(Environment.CurrentDirectory).FullName : Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            var absolutePath = Path.Combine(basePath, "src", "Resources", "sampleFile.txt");
            
            //Construction
            var scriptProvider = new FileAxeScriptProvider(absolutePath);
            scriptProvider.Should().NotBeNull();

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

        //[Test]
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