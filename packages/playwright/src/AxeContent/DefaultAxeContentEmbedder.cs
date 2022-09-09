#nullable enable

using Deque.AxeCore.Commons;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright.AxeContent
{
    /// <inheritdoc />
    internal sealed class DefaultAxeContentEmbedder : IAxeContentEmbedder
    {
        private readonly IAxeScriptProvider m_axeScriptProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultAxeContentEmbedder(IAxeScriptProvider axeScriptProvider)
        {
            m_axeScriptProvider = axeScriptProvider;
        }

        /// <inheritdoc />
        public async Task EmbedAxeCoreIntoPage(IPage page, bool? embedIntoIFrames = true)
        {
            string axeCoreScriptContent = m_axeScriptProvider.GetScript();

            if (!embedIntoIFrames.HasValue || embedIntoIFrames.Value)
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
