using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class AxeResultTest
    {
        readonly string basicAxeResultJson = @"
{
  ""testEngine"": {
    ""name"": ""axe-core"",
    ""version"": ""4.4.1""
  },
  ""testRunner"": {
    ""name"": ""axe""
  },
  ""testEnvironment"": {
    ""userAgent"": ""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/100.0.4889.0 Safari/537.36"",
    ""windowWidth"": 800,
    ""windowHeight"": 600,
    ""orientationAngle"": 0,
    ""orientationType"": ""portrait-primary""
  },
  ""timestamp"": ""2000-01-02T03:04:05.006Z"",
  ""url"": ""http://localhost/"",
  ""toolOptions"": {
    ""xpath"": true,
    ""runOnly"": {
      ""type"": ""rule"",
      ""values"": [
        ""document-title""
      ]
    },
    ""reporter"": ""v1""
  },
  ""inapplicable"": [],
  ""passes"": [],
  ""incomplete"": [],
  ""violations"": [
    {
      ""id"": ""document-title"",
      ""impact"": ""serious"",
      ""tags"": [
        ""cat.text-alternatives"",
        ""wcag2a"",
        ""wcag242"",
        ""ACT""
      ],
      ""description"": ""Ensures each HTML document contains a non-empty <title> element"",
      ""help"": ""Documents must have <title> element to aid in navigation"",
      ""helpUrl"": ""https://dequeuniversity.com/rules/axe/4.4/document-title?application=axe-puppeteer"",
      ""nodes"": [
        {
          ""any"": [
            {
              ""id"": ""doc-has-title"",
              ""data"": null,
              ""relatedNodes"": [],
              ""impact"": ""serious"",
              ""message"": ""Document does not have a non-empty <title> element""
            }
          ],
          ""all"": [],
          ""none"": [],
          ""impact"": ""serious"",
          ""html"": ""<html><head></head><body>\n</body></html>"",
          ""target"": [
            ""html""
          ],
          ""xpath"": [
            ""/html""
          ],
          ""failureSummary"": ""Fix any of the following:\n  Document does not have a non-empty <title> element""
        }
      ]
    }
  ]
}
        ";

        [Test]
        public void CtorShouldThrowExceptionForErrorProperty()
        {
            Assert.Throws<Exception>(() => new AxeResult(JObject.FromObject(JsonConvert.DeserializeObject(@"
            {
                error: ""message from JSON error property""
            }
            "))), "JavaScript error occurred while running axe-core in page: message from JSON error property");
        }

        [Test]
        public void CtorShouldParseFieldsFromBasicSampleAxeResultJson()
        {
            var result = new AxeResult(JObject.FromObject(JsonConvert.DeserializeObject(basicAxeResultJson)));

            result.TestEngineName.Should().Be("axe-core");
            result.TestEngineVersion.Should().Be("4.4.1");
            result.TestRunner.Name.Should().Be("axe");
            result.TestEnvironment.UserAgent.Should().Be("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/100.0.4889.0 Safari/537.36");
            result.TestEnvironment.WindowWidth.Should().Be(800);
            result.TestEnvironment.WindowHeight.Should().Be(600);
            result.TestEnvironment.OrientationAngle.Should().Be(0);
            result.TestEnvironment.OrientationType.Should().Be("portrait-primary");
            result.Timestamp.Should().Be(new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.Zero));
            result.Url.Should().Be("http://localhost/");
            result.ToolOptions.ToString().Should().NotBeNullOrEmpty();
            result.Inapplicable.Should().BeEmpty();
            result.Passes.Should().BeEmpty();
            result.Incomplete.Should().BeEmpty();
            result.Violations.Should().HaveCount(1);

            var violation = result.Violations.Single();
            violation.Id.Should().Be("document-title");
            violation.Impact.Should().Be("serious");
            violation.Tags.Should().BeEquivalentTo("cat.text-alternatives", "wcag2a", "wcag242", "ACT");
            violation.Description.Should().Be("Ensures each HTML document contains a non-empty <title> element");
            violation.Help.Should().Be("Documents must have <title> element to aid in navigation");
            violation.HelpUrl.Should().Be("https://dequeuniversity.com/rules/axe/4.4/document-title?application=axe-puppeteer");
            violation.Nodes.Should().HaveCount(1);

            var node = violation.Nodes.Single();
            node.All.Should().BeEmpty();
            node.None.Should().BeEmpty();
            node.Impact.Should().Be("serious");
            node.Html.Should().Be("<html><head></head><body>\n</body></html>");
            node.Target.Should().Be(new AxeSelector("html"));
            node.XPath.Should().Be(new AxeSelector("/html"));
            node.Any.Should().HaveCount(1);

            var check = node.Any.Single();
            check.Id.Should().Be("doc-has-title");
            check.Data.Should().BeNull();
            check.RelatedNodes.Should().BeEmpty();
            check.Impact.Should().Be("serious");
            check.Message.Should().Be("Document does not have a non-empty <title> element");
        }

        [Test]
        public void ToStringShouldBeStable()
        {
            var result1 = new AxeResult(JObject.FromObject(JsonConvert.DeserializeObject(basicAxeResultJson)));
            var result2 = new AxeResult(JObject.FromObject(JsonConvert.DeserializeObject(basicAxeResultJson)));

            result1.ToString().Should().Be(result2.ToString());
        }

        [Test]
        public void ToStringShouldIncludeEnoughInfoToBeActionable()
        {
            var result = new AxeResult(JObject.FromObject(JsonConvert.DeserializeObject(basicAxeResultJson)));
            var toStringOutput = result.ToString();

            toStringOutput.Should().ContainAll(new string[] {
                "4.4.1",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/100.0.4889.0 Safari/537.36",
                "2000-01-02T03:04:05.006+00:00",
                "http://localhost/",
                "document-title",
                "wcag242",
                "serious",
                "Ensures each HTML document contains a non-empty <title> element",
                "https://dequeuniversity.com/rules/axe/4.4/document-title?application=axe-puppeteer",
                "<html><head></head><body>",
                "</body></html>",
                "/html",
                "doc-has-title",
                "Document does not have a non-empty <title> element",
            });
        }
    }
}
