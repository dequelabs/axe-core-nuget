#nullable enable

using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Playwright.Axe.AxeContent
{
    /// <inheritdoc />
    internal sealed class DefaultAxeContentEmbedder : IAxeContentEmbedder
    {
        private readonly IAxeContentProvider m_axeContentProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultAxeContentEmbedder(IAxeContentProvider axeContentProvider)
        {
            m_axeContentProvider = axeContentProvider;
        }

        /// <inheritdoc />
        public async Task EmbedAxeCoreIntoPage(IPage page, bool? embedIntoIFrames = true)
        {
            string axeCoreScriptContent = m_axeContentProvider.GetAxeCoreScriptContent();

            if(!embedIntoIFrames.HasValue || embedIntoIFrames.Value)
            {
                foreach (IFrame frame in page.Frames)
                {
                    await frame.EvaluateAsync($"() => {axeCoreScriptContent}");
                }
            }
            else
            {
                await page.EvaluateAsync($"() => {axeCoreScriptContent}");
            }
        }
    }
}
