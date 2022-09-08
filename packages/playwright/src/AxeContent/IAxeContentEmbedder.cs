#nullable enable

using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright.AxeContent
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
