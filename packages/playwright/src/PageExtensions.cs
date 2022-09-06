#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Deque.AxeCore.Playwright.AxeCoreWrapper;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Extensions for adding Axe to Playwright Page.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Retrieves rule known in Axe.
        /// </summary>
        /// <param name="page">The Playwright Page object.</param>
        /// <param name="tags">Array of tags used to filter returned rules.</param>
        /// <returns>List of <see cref="AxeRuleMetadata"/></returns>
        public static async Task<IList<AxeRuleMetadata>> GetAxeRules(this IPage page, IList<string>? tags = null)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.GetRules(page, tags);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(this IPage page, AxeRunOptions? options = null)
        {
            return await RunAxeInner(page, null, options);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="context">Context to specify which element to run axe on.</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(
            this IPage page, 
            AxeRunContext context, 
            AxeRunOptions? options = null)
        {
            return await RunAxeInner(page, context, options);
        }

        private static async Task<AxeResults> RunAxeInner(this IPage page, AxeRunContext? context, AxeRunOptions? options)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            AxeResults results = await axeCoreWrapper.Run(page, context, options);
            IFileSystem fileSystem = new FileSystem();

            return results;
        }
    }
}
