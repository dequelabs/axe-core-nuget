#nullable enable

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
    }
}
