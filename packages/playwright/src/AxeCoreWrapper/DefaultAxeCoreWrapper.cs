#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright.AxeCoreWrapper
{
    /// <inheritdoc/>
    internal class DefaultAxeCoreWrapper : IAxeCoreWrapper
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new RunContextJsonConverter()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private readonly IAxeContentEmbedder m_axeContentEmbedder;

        public DefaultAxeCoreWrapper(IAxeContentEmbedder axeContentEmbedder)
        {
            m_axeContentEmbedder = axeContentEmbedder;
        }

        /// <inheritdoc/>
        public async Task<IList<AxeRuleMetadata>> GetRules(IPage page, IList<string>? tags = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(page, false);

            object jsonObject = await page.EvaluateAsync<object>("(paramTags) => window.axe.getRules(paramTags)", tags);
            return DeserializeRuleMetadata(jsonObject);
        }

        /// <inheritdoc/>
        public async Task<AxeResult> Run(IPage page, AxeRunContext? context = null, AxeRunOptions? options = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(page, options?.Iframes);

            AxeResult axeResult = await EvaluateAxeRun(page, context, options);

            return axeResult;
        }

        /// <inheritdoc/>
        public async Task<AxeResult> RunOnLocator(ILocator locator, AxeRunOptions? options = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(locator.Page, options?.Iframes);

            string paramString = JsonConvert.SerializeObject(options);
            string runParamTemplate = options != null ? "JSON.parse(runOptions)" : string.Empty;

            object jsonObject = await locator.EvaluateAsync<object>($"(node, runOptions) => window.axe.run(node, {runParamTemplate})", paramString);
            return DeserializeResult(jsonObject);
        }

        private static async Task<AxeResult> EvaluateAxeRun(IPage page, AxeRunContext? context = null, object? param = null)
        {
            string? paramString = JsonConvert.SerializeObject(param);

            string runParamTemplate = param != null ? "JSON.parse(runOptions)" : string.Empty;

            string? contextParam = context is null ? string.Empty : ($"JSON.parse(\'{JsonConvert.SerializeObject(context)}\'),");

            object jsonObject = await page.EvaluateAsync<object>($"(runOptions) => window.axe.run({contextParam}{runParamTemplate})", paramString);

            return DeserializeResult(jsonObject);
        }

        private static AxeResult DeserializeResult(object jsonObject)
        {
            if (jsonObject is null)
            {
                throw new Exception();
            }

            JObject jObject = JObject.FromObject(jsonObject);

            return new AxeResult(jObject);
        }

        private static List<AxeRuleMetadata> DeserializeRuleMetadata(object jsonObject)
        {
            string result = JsonConvert.SerializeObject(jsonObject);
            var results = JsonConvert.DeserializeObject<List<AxeRuleMetadata>>(result);

            if (results is null)
            {
                throw new Exception();
            }

            return results;
        }
    }
}
