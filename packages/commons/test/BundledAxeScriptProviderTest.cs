using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Deque.AxeCore.Commons;
using Moq;
using FluentAssertions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class BundledAxeScriptProviderTest
    {
        private static readonly string testContext = TestContext.CurrentContext.TestDirectory.ToString();
        private readonly string basePath = testContext.Split(new string[] { "test" }, StringSplitOptions.None)[0];
        private static readonly string copyrightHeader = "* Your use of this Source Code Form is subject to the terms of the Mozilla Public\n * License, v. 2.0.";
        [Test]
        public void GetScriptCalled()
        {
            var scriptProvider = new BundledAxeScriptProvider();
            Assert.DoesNotThrow(() =>
            {
                string readResult = scriptProvider.GetScript();
                string axeVersion = "axe v" + getAxeVersion();

                readResult.Should().NotBeNull();
                Assert.That(readResult, Contains.Substring(axeVersion));
                Assert.That(readResult, Contains.Substring(copyrightHeader));
            });
        }

        private string getAxeVersion()
        {
            string absolutePackageJsonPath = Path.Combine(basePath, "src", "package.json");
            JObject packageJson = JObject.Parse(File.ReadAllText(absolutePackageJsonPath));
            string axeVersion = (string)packageJson["dependencies"]["axe-core"];
            string regex = "(\\d+\\.)?(\\d+\\.)?(\\d+)";
            axeVersion = Regex.Match(axeVersion, regex).ToString();

            return axeVersion;
        }
    }
}
