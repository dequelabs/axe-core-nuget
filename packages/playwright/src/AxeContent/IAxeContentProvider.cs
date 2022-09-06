#nullable enable

using System.Collections.Generic;

namespace Deque.AxeCore.Playwright.AxeContent
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
