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
        
        //Tests for creating a FileAxeScriptProvider
        [Test]
        public void ConstructorPassedValidFile()
        {
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
                var scriptProvider = new FileAxeScriptProvider("sample.html");
                scriptProvider.Should().NotBeNull();
            });
        }

        // [Test]
        // public void GetScriptForNonexistentFile()
        // {
        //     //Add a file to the directory
        //     var newPath = Path.Combine(basePath, "src", "Resources", "foo.txt");
        //     Console.WriteLine(newPath);
        //     File.Create(newPath);
        //     //Create a FileAxeScriptProvider
        //     var scriptProvider = new FileAxeScriptProvider(newPath);
        //     //Remove the file from the directory
        //     File.Delete(newPath);
        //     //Call GetScript and Assert the InvalidOperationException is thrown
        //     Assert.Throws<InvalidOperationException>(() =>
        //     {
        //         scriptProvider.GetScript();
        //         scriptProvider.Should().NotBeNull();
        //     });
        // }
            
    }
}