#nullable enable

using System.Threading.Tasks;

namespace Playwright.Axe.HtmlReport
{
    /// <summary>
    /// Builds Html Reports for
    /// </summary>
    internal interface IHtmlReportBuilder
    {
        /// <summary>
        /// Builds and writes a Html Report for a Run
        /// </summary>
        /// <param name="runResults">The results of the run.</param>
        /// <param name="options">Options for running.</param>
        public void BuildReport(AxeResults runResults, AxeHtmlReportOptions options);
    }
}
