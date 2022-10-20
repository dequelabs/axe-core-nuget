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
        public Task EmbedAxeCoreIntoPage(IPage page, bool? embedIntoIFrames = true, bool setAllowedOrigin = false);

        /// <summary>
        /// Embeds Axe Core into Frame
        /// </summary>
        public Task EmbedAxeCoreIntoFrame(IFrame page);
    }
}
