#nullable enable

using System.Collections.Generic;

namespace Playwright.Axe.AxeContent
{
    /// <summary>
    /// Provides Axe File Content
    /// </summary>
    public interface IAxeContentProvider
    {
        /// <summary>
        /// Retrieves the Axe Core library file content.
        /// </summary>
        public string GetAxeCoreScriptContent();

        /// <summary>
        /// Retrieves the static files for creating the Html Report.
        /// </summary>
        public IDictionary<string, string> GetHtmlReportFiles();
    }
}
