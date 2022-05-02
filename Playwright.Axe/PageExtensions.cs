#nullable enable

using Microsoft.Playwright;
using Playwright.Axe.AxeContent;
using Playwright.Axe.AxeCoreWrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playwright.Axe
{
    /// <summary>
    /// Extensions for adding Axe to Playwright Page.
    /// </summary>
    public static class PageExtensions
    {
        public static async Task<IList<AxeRuleMetadata>> GetAxeRules(this IPage page, IList<string>? tags = null)
        {
            IAxeContentProvider axeContentProvider = new DefaultAxeContentProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeContentProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.GetRules(page, tags);
        }

        public static async Task<AxeResults> RunAxe(this IPage page, AxeRunOptions? options = null)
        {
            IAxeContentProvider axeContentProvider = new DefaultAxeContentProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeContentProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.Run(page, options);
        }
    }
}
