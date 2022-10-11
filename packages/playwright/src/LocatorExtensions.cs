#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Deque.AxeCore.Playwright.AxeCoreWrapper;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Extensions for adding Axe to Playwright Locator.
    /// </summary>
    public static class LocatorExtensions
    {
        /// <summary>
        /// Runs Axe against the selected elements from a locator.
        /// </summary>
        /// <param name="locator">The Playwright Locator</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResult> RunAxe(this ILocator locator, AxeRunOptions? options = null)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.RunOnLocator(locator, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs Axe against the selected elements from a locator.
        /// </summary>
        /// <param name="locator">The Playwright Locator</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        [Obsolete("Legacy Mode is being removed in the future. Use with caution!")]
        public static async Task<AxeResult> RunAxeLegacy(this ILocator locator, AxeRunOptions? options = null)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.RunLegacyOnLocator(locator, options).ConfigureAwait(false);
        }
    }
}
