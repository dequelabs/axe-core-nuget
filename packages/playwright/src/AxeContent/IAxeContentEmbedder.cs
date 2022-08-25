#nullable enable

using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Playwright.Axe.AxeContent
{
    /// <summary>
    /// Embeds Axe Content.
    /// </summary>
    internal interface IAxeContentEmbedder
    {
        /// <summary>
        /// Embeds Axe Core into Page
        /// </summary>
        public Task EmbedAxeCoreIntoPage(IPage page, bool? embedIntoIFrames = true);
    }
}
