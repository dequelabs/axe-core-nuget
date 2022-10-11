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
