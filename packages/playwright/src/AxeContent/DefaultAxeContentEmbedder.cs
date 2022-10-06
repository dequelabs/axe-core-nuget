#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
        public async Task EmbedAxeCoreIntoPage(IPage page, bool? embedIntoIFrames = true, bool setAllowOrigin = false)
        {
            string axeCoreScriptContent = m_axeScriptProvider.GetScript();

            if (!embedIntoIFrames.HasValue || embedIntoIFrames.Value)
            {
                foreach (IFrame frame in page.Frames)
                {
                    await frame.EvaluateAsync($"() => {axeCoreScriptContent}").ConfigureAwait(false);
                    await ConfigureAxeInFrame(frame, setAllowOrigin);
                }
            }
            else
            {
                await page.EvaluateAsync($"() => {axeCoreScriptContent}").ConfigureAwait(false);
                await ConfigureAxeInFrame(page.MainFrame, setAllowOrigin);
            }
        }

        public async Task ConfigureAxeInFrame(IFrame frame, bool setAllowedOrigin)
        {

            var runPartialExists = await frame.EvaluateAsync<bool>(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("runPartialExists.js"));

            if (!runPartialExists && setAllowedOrigin)
            {
                await frame.EvaluateAsync(
                    await EmbeddedResourceProvider.ReadEmbeddedFileAsync("allowIframeUnsafe.js")
                );
            }
            await frame.EvaluateAsync(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("branding.js"));
        }

        /// <inheritdoc />
        public async Task EmbedAxeCoreIntoFrame(IFrame frame)
        {
            string axeCoreScriptContent = m_axeScriptProvider.GetScript();
            await frame.EvaluateAsync($"() => {axeCoreScriptContent}");
        }
    }
}
