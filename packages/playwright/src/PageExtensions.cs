#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Deque.AxeCore.Playwright.AxeCoreWrapper;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
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

            return await axeCoreWrapper.GetRules(page, tags).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResult</returns>
        public static async Task<AxeResult> RunAxe(this IPage page, AxeRunOptions? options = null)
        {
            return await RunAxeInner(page, null, options, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="context">Context to specify which element to run axe on.</param>
        /// <param name="options">Options for running Axe.</param>
        /// <param name="axeSource">Source code for axe-core</param>
        /// <returns>The AxeResult</returns>
        public static async Task<AxeResult> RunAxe(
            this IPage page,
            AxeRunContext context,
            AxeRunOptions? options = null,
            string? axeSource = null)
        {
            return await RunAxeInner(page, context, options, axeSource).ConfigureAwait(false);
        }

        private static async Task<AxeResult> RunAxeInner(this IPage page, AxeRunContext? context, AxeRunOptions? options, string? axeSource)
        {
            IAxeScriptProvider axeScriptProvider;
            if (axeSource == null)
            {
                axeScriptProvider = new BundledAxeScriptProvider();
            }
            else
            {
                axeScriptProvider = new StringAxeScriptProvider(axeSource);
            }
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            AxeResult results = await axeCoreWrapper.Run(page, context, options).ConfigureAwait(false);
            IFileSystem fileSystem = new FileSystem();

            return results;
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="context">Context to specify which element to run axe on.</param>
        /// <param name="options">Options for running Axe.</param>
        /// <param name="axeSource">Source code for axe-core</param>
        /// <returns>The AxeResult</returns>
        [Obsolete("Legacy Mode is being removed in the future. Use with caution!")]
        public static async Task<AxeResult> RunAxeLegacy(
            this IPage page,
            AxeRunContext context,
            AxeRunOptions? options = null,
            string? axeSource = null)
        {
            return await RunAxeLegacyInner(page, context, options, axeSource);
        }

        private static async Task<AxeResult> RunAxeLegacyInner(this IPage page, AxeRunContext? context, AxeRunOptions? options, string? axeSource)
        {
            IAxeScriptProvider axeScriptProvider;
            if (axeSource == null)
            {
                axeScriptProvider = new BundledAxeScriptProvider();
            }
            else
            {
                axeScriptProvider = new StringAxeScriptProvider(axeSource);
            }
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

#pragma warning disable CS0618
            AxeResult results = await axeCoreWrapper.RunLegacy(page, context, options);
#pragma warning restore CS0618
            IFileSystem fileSystem = new FileSystem();

            return results;
        }
    }
}
