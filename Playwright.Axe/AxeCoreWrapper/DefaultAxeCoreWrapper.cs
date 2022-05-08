#nullable enable

using Microsoft.Playwright;
using Playwright.Axe.AxeContent;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Playwright.Axe.AxeCoreWrapper
{
    /// <inheritdoc/>
    internal sealed class DefaultAxeCoreWrapper : IAxeCoreWrapper
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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

            string? jsonString = (await page.EvaluateAsync("(paramTags) => JSON.stringify(window.axe.getRules(paramTags))", tags))
                .Value.GetString();
            
            if(jsonString == null)
            {
                throw new Exception();
            }

            IList<AxeRuleMetadata>? ruleMetadatas = JsonSerializer.Deserialize<List<AxeRuleMetadata>>(jsonString, s_jsonOptions);
            if (ruleMetadatas == null)
            {
                throw new Exception();
            }

            return ruleMetadatas;
        }

        /// <inheritdoc/>
        public async Task<AxeResults> Run(IPage page, AxeRunOptions? options = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(page, options?.Iframes);

            AxeResults? axeResult = await EvaluateAxeRun<AxeResults>(page, options);

            if(axeResult == null)
            {
                throw new Exception();
            }

            return axeResult;
        }

        /// <inheritdoc/>
        public async Task<AxeResults> RunOnLocator(ILocator locator, AxeRunOptions? options = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(locator.Page, options?.Iframes);

            string? paramString = JsonSerializer.Serialize(options, s_jsonOptions);
            string runParamTemplate = options != null ? "JSON.parse(runOptions)" : string.Empty;

            var resultJsonElement = await locator.EvaluateAsync($"(node, runOptions) => window.axe.run(node, {runParamTemplate})", paramString);
            return DeserializeAxeResults(resultJsonElement);
        }

        private static async Task<TResult?> EvaluateAxeRun<TResult>(IPage page, object? param = null)
            where TResult : class
        {
            string? paramString = JsonSerializer.Serialize(param, s_jsonOptions);
            string runParamTemplate = param != null ? "JSON.parse(runOptions)" : string.Empty;

            JsonElement? resultJsonElement = await page.EvaluateAsync($"(runOptions) => window.axe.run({runParamTemplate})", paramString);

            if (!resultJsonElement.HasValue)
            {
                return null;
            }

            TResult? result = JsonSerializer.Deserialize<TResult>(resultJsonElement.Value, s_jsonOptions);
            return result;
        }

        private static AxeResults DeserializeAxeResults(JsonElement? jsonElement)
        {
            if (!jsonElement.HasValue)
            {
                throw new Exception();
            }

            AxeResults? results = JsonSerializer.Deserialize<AxeResults>(jsonElement.Value, s_jsonOptions);

            if (results is null)
            {
                throw new Exception();
            }

            return results;
        }
    }
}
