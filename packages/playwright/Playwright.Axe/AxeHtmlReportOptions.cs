#nullable enable

namespace Playwright.Axe
{
    /// <summary>
    /// Options for the creation of a Html Report of an Axe Run
    /// </summary>
    public sealed class AxeHtmlReportOptions
    {
        /// <summary>
        /// The directory to write the output files for this report.
        /// There may be multiple files produced for a report.
        /// </summary>
        public string? ReportDir { get; }

        /// <summary>
        /// Only create if violations are present in the run.
        /// </summary>
        public bool OnlyOnViolations { get; }

        /// <summary>
        /// Options for creating an Axe Html Report.
        /// </summary>
        public AxeHtmlReportOptions(string? reportDir = null, bool? onlyOnViolations = true)
        {
            ReportDir = reportDir;
            OnlyOnViolations = onlyOnViolations!.Value;
        }
    }
}
