#nullable enable

using Playwright.Axe.AxeContent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playwright.Axe.HtmlReport
{
    /// <inheritdoc />
    internal sealed class HtmlReportBuilder : IHtmlReportBuilder
    {
        private readonly IAxeContentProvider m_contentProvider;

        private const string ResultsDataFileName = "data.js";

        public HtmlReportBuilder(IAxeContentProvider contentProvider)
        {
            m_contentProvider = contentProvider;
        }

        /// <inheritdoc />
        public void BuildReport(AxeResults runResults, AxeHtmlReportOptions options)
        {
            IDictionary<string, string> staticFiles = m_contentProvider.GetHtmlReportFiles();
            staticFiles.Add(ResultsDataFileName, GetResultsDataFileContent(runResults));

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

        private static string GetResultsDataFileContent(AxeResults runResults)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string resultsData = JsonSerializer.Serialize(runResults, serializerOptions);
            string fileData = $"window.AxeResults = {resultsData}";

            return fileData;
        }
    }
}
