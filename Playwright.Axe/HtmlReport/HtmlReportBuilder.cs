#nullable enable

using Playwright.Axe.AxeContent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playwright.Axe.HtmlReport
{
    /// <inheritdoc />
    internal sealed class HtmlReportBuilder : IHtmlReportBuilder
    {
        private readonly IAxeContentProvider m_contentProvider;

        private readonly IDirectory m_directory;

        private readonly IFile m_file;

        private const string ResultsDataFileName = "data.js";

        private const string DefaultReportDirectory = "report";

        public HtmlReportBuilder(
            IAxeContentProvider contentProvider,
            IDirectory directory,
            IFile file)
        {
            m_contentProvider = contentProvider;
            m_directory = directory;
            m_file = file;
        }

        /// <inheritdoc />
        public void BuildReport(AxeResults runResults, AxeHtmlReportOptions options)
        {
            IDictionary<string, string> staticFiles = m_contentProvider.GetHtmlReportFiles();
            staticFiles.Add(ResultsDataFileName, GetResultsDataFileContent(runResults));

            string reportDirectory = options.ReportDir ?? DefaultReportDirectory;

            WriteFiles(reportDirectory, staticFiles);
        }

        private void WriteFiles(string outputDir, IDictionary<string, string> files)
        {
            foreach(var file in files)
            {
                string filePath = Path.Combine(outputDir, file.Key);

                if(!m_directory.Exists(outputDir))
                {
                    m_directory.CreateDirectory(outputDir);
                }

                m_file.WriteAllText(filePath, file.Value);
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
