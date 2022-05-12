#nullable enable

using Microsoft.Playwright;
using Playwright.Axe.AxeContent;
using Playwright.Axe.AxeCoreWrapper;
using Playwright.Axe.HtmlReport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playwright.Axe
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
            IAxeContentProvider axeContentProvider = new DefaultAxeContentProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeContentProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.GetRules(page, tags);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="options">Options for running Axe.</param>
        /// <param name="reportOptions">Options for creating a Html report of the run.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(this IPage page, AxeRunOptions? options = null, AxeHtmlReportOptions? reportOptions = null)
        {
            return await RunAxeInner(page, null, options, reportOptions);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="context">Context to specify which element to run axe on.</param>
        /// <param name="options">Options for running Axe.</param>
        /// <param name="reportOptions">Options for creating a Html report of the run.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(
            this IPage page, 
            AxeRunContext context, 
            AxeRunOptions? options = null, 
            AxeHtmlReportOptions? reportOptions = null)
        {
            return await RunAxeInner(page, context, options, reportOptions);
        }

        private static async Task<AxeResults> RunAxeInner(this IPage page, AxeRunContext? context, AxeRunOptions? options, AxeHtmlReportOptions? reportOptions)
        {
            IAxeContentProvider axeContentProvider = new DefaultAxeContentProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeContentProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            AxeResults results = await axeCoreWrapper.Run(page, context, options);

            if(reportOptions != null)
            {
                IHtmlReportBuilder htmlReportBuilder = new HtmlReportBuilder(axeContentProvider);
                htmlReportBuilder.BuildReport(results, reportOptions);
            }

            return results;
        }
    }
}
