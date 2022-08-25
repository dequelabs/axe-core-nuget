#nullable enable

using Deque.AxeCore.Playwright.AxeContent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Deque.AxeCore.Playwright.HtmlReport;

namespace Deque.AxeCore.Playwright.Test
{
    [TestClass]
    public sealed class HtmlReportBuilderTests
    {
        [TestMethod]
        [DynamicData(nameof(CreateReportDirectoryTestData), DynamicDataSourceType.Method)]
        public void BuildReport_WithReportDirectoryOptions_ShouldWriteExpectedFiles(string? reportDir, List<string> expectedFiles)
        {
            Mock<IAxeContentProvider> contentProvider = new ();
            Mock<IFile> file = new();
            Mock<IDirectory> directory = new();

            IDictionary<string, string> htmlReportFiles = new Dictionary<string, string>()
            {
                { "index.html", "<a>index</a>" },
                { "index.js", "() => void" }
            };

            contentProvider.Setup(cp => cp.GetHtmlReportFiles())
                .Returns(htmlReportFiles);

            expectedFiles.ForEach(expectedFile =>
            {
                file.Setup(mock => mock.WriteAllText(expectedFile, It.IsNotNull<string>()))
                    .Verifiable();
            });

            HtmlReportBuilder htmlReportBuilder = new(contentProvider.Object, directory.Object, file.Object);

            AxeHtmlReportOptions htmlReportOptions = new(reportDir);
            AxeResults results = new(null!, null!, null!, null!, DateTime.UtcNow, new List<AxeResult>(), new List<AxeResult>(), new List<AxeResult>(), new List<AxeResult>());

            htmlReportBuilder.BuildReport(results, htmlReportOptions);
            Mock.Verify(file);
        }

        private static IEnumerable<object?[]> CreateReportDirectoryTestData()
        {
            yield return new object?[]
            {
                null,
                new List<string> { "report\\index.html", "report\\index.js", "report\\data.js" }
            };

            yield return new object[]
            {
                "C:\\\\users\\my-custom-report-dir",
                new List<string> { "C:\\\\users\\my-custom-report-dir\\index.html", "C:\\\\users\\my-custom-report-dir\\index.js", "C:\\\\users\\my-custom-report-dir\\data.js" }
            };
        }
    }
}
