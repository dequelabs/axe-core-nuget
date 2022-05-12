#nullable enable

using Playwright.Axe.AxeContent;
using System.Collections.Generic;
using System.IO;

namespace Playwright.Axe.HtmlReport
{
    /// <inheritdoc />
    internal sealed class HtmlReportBuilder : IHtmlReportBuilder
    {
        private readonly IAxeContentProvider m_contentProvider;

        public HtmlReportBuilder(IAxeContentProvider contentProvider)
        {
            m_contentProvider = contentProvider;
        }

        /// <inheritdoc />
        public void BuildReport(AxeResults runResults, AxeHtmlReportOptions options)
        {
            IDictionary<string, string> staticFiles = m_contentProvider.GetHtmlReportFiles();

            WriteFiles("report", staticFiles);
        }

        private void WriteFiles(string outputDir, IDictionary<string, string> files)
        {
            foreach(var file in files)
            {
                string filePath = Path.Combine(outputDir, file.Key);

                if(!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                File.WriteAllText(filePath, file.Value);
            }
        }
    }
}
