#nullable enable

using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playwright.Axe.AxeCoreWrapper
{
    /// <summary>
    /// Wrapper for Axe Core Library
    /// </summary>
    public interface IAxeCoreWrapper
    {
        /// <summary>
        /// Gets Rule Metadata based on tags.
        /// </summary>
        public Task<IList<AxeRuleMetadata>> GetRules(IPage page, IList<string>? tags = null);

        /// <summary>
        /// Calls the Run/Analyze method of Axe Core.
        /// </summary>
        public Task<AxeResults> Run(IPage page, AxeRunOptions? options = null);
    }
}
