#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright.AxeCoreWrapper
{
    /// <inheritdoc/>
    internal class DefaultAxeCoreWrapper : IAxeCoreWrapper
    {

        private readonly IAxeContentEmbedder m_axeContentEmbedder;
        private static readonly DefaultContractResolver camelCaseContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        private static readonly JsonSerializerSettings JsonSerializerSettingsFinishRun = new JsonSerializerSettings
        {
            ContractResolver = camelCaseContractResolver,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include
        };

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
            var rawContextArg = JsonConvert.SerializeObject(context);
            AxeResult axeResult = await RunInner(page.MainFrame, rawContextArg, options);

            return axeResult;
        }

        /// <inheritdoc/>
        public async Task<AxeResult> RunOnLocator(ILocator locator, AxeRunOptions? options = null)
        {
            var frame = await (await locator.ElementHandleAsync()).OwnerFrameAsync();
            if (frame == null)
            {
                throw new Exception("Could not locate frame associated with locator");
            }
            return await RunInner(frame, await locator.ElementHandleAsync(), options);
        }

        /// <inheritdoc/>
        public async Task<AxeResult> RunLegacy(IPage page, AxeRunContext? context = null, AxeRunOptions? options = null)
        {
            var rawContextArg = JsonConvert.SerializeObject(context);
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(page, options?.Iframes);

            AxeResult axeResult = await EvaluateAxeRun(page, rawContextArg, options);

            return axeResult;
        }

        /// <inheritdoc/>
        public async Task<AxeResult> RunLegacyOnLocator(ILocator locator, AxeRunOptions? options = null)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoPage(locator.Page, options?.Iframes);

            string? paramString = JsonConvert.SerializeObject(options);
            string runParamTemplate = options != null ? "JSON.parse(runOptions)" : string.Empty;

            object jsonObject = await locator.EvaluateAsync<object>($"(node, runOptions) => window.axe.run(node, {runParamTemplate})", paramString);
            return DeserializeResult(jsonObject);
        }

        private static async Task<AxeResult> EvaluateAxeRun(IPage page, string? context = null, object? param = null)
        {
            string? paramString = JsonConvert.SerializeObject(param);

            string runParamTemplate = param != null ? "JSON.parse(runOptions)" : string.Empty;

            string? contextParam = context is null ? string.Empty : ($"JSON.parse(\'{context}\'),");

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

        private async Task<AxeResult> RunInner(IFrame frame, object rawContextArg, AxeRunOptions? runOptions)
        {
            await ConfigureAxe(frame);


            if (runOptions == null)
            {
                runOptions = new AxeRunOptions
                {
                    Iframes = true
                };
            }

            var runPartialExists = await RunPartialExists(frame);

            if (!runPartialExists)
            {
                if (rawContextArg is ILocator)
                {
                    return await RunLegacyOnLocator((ILocator) rawContextArg, runOptions);
                }
                else if (rawContextArg is string)
                {

                    await m_axeContentEmbedder.EmbedAxeCoreIntoPage(frame.Page, runOptions.Iframes, true);
                    return await EvaluateAxeRun(frame.Page, (string) rawContextArg, runOptions);
                }
                else
                {
                    throw new ArgumentException("Context arg is of invalid type");
                }
            }
            else
            {
                return await AnalyzeAxeRunPartial(frame, rawContextArg, runOptions);
            }


        }

        private async Task<AxeResult> AnalyzeAxeRunPartial(IFrame frame, object rawContextArg, AxeRunOptions runOptions)
        {
            string rawOptionsArg = JsonConvert.SerializeObject(runOptions);
            var partialResults = await RunPartialRecursive(frame, rawOptionsArg, rawContextArg, true, !runOptions.Iframes.HasValue || runOptions.Iframes.Value);
            return await IsolatedFinishRun(frame, partialResults.ToArray(), rawOptionsArg);
        }

        private async Task<List<object?>> RunPartialRecursive(
                                                        IFrame frame,
                                                        object options,
                                                        object context,
                                                        bool isTopLevel,
                                                        bool iframes
                                                        )
        {
            if (!isTopLevel)
            {
                await ConfigureAxe(frame);
            }

            var partialResults = new List<object?>();

            try
            {
                string partialRes = await frame.EvaluateAsync<string>(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("runPartial.js"), new[] { context, options });
                // Important to deserialize because we want to reserialize as an
                // array of object, not an array of strings.
                partialResults.Add(JsonConvert.DeserializeObject<object>(partialRes));
            }
            catch (Exception ex)
            {
                if (isTopLevel)
                {
                    throw;
                }
                else
                {
                    partialResults.Add(null);
                    return partialResults;
                }
            }

            // Don't go any deeper if we are just doing top-level iframe
            if (!iframes)
            {
                return partialResults;
            }

            var frameContexts = await GetFrameContexts(frame, context);
            foreach (var fContext in frameContexts)
            {
                try
                {
                    object frameContext = JsonConvert.SerializeObject(fContext.Context);
                    string frameSelector = JsonConvert.SerializeObject(fContext.Selector);
                    var frameHandle = await frame.EvaluateHandleAsync(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("shadowSelect.js"), frameSelector);
                    if (frameHandle != null)
                    {
                        var childFrameElement = frameHandle.AsElement();
                        if (childFrameElement == null)
                        {
                            partialResults.Add(null);
                            continue;
                        }

                        var childFrame = await childFrameElement.ContentFrameAsync();
                        if (childFrame == null)
                        {
                            partialResults.Add(null);
                            continue;
                        }

                        partialResults.AddRange(await RunPartialRecursive(childFrame, options, frameContext, false, iframes));
                    }
                    else
                    {
                        Console.WriteLine("Frame Handle is null!!!!");
                        partialResults.Add(null);
                    }
                }
                catch (Exception)
                {
                    partialResults.Add(null);
                }
            }

            return partialResults;
        }

        private async Task<AxeResult> IsolatedFinishRun(IFrame frame, object?[] partialResults, object options)
        {
            var browser = frame.Page.Context.Browser;
            if (browser == null)
            {
                throw new Exception("Could not locate browser associated with page");
            }
            var blankPage = await browser.NewPageAsync();
            try
            {
                await ConfigureAxe(blankPage.MainFrame);
                var serializedPartials = JsonConvert.SerializeObject(partialResults, JsonSerializerSettingsFinishRun);
                var result = await blankPage.EvaluateAsync<object>(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("finishRun.js"), new[] { serializedPartials, options });
                return DeserializeResult(result);
            }
            finally
            {
                await blankPage.CloseAsync();
            }
        }

        /// <summary>
        /// Execute `axe.utils.getFrameContexts(context)`
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<List<AxeFrameContext>> GetFrameContexts(IFrame frame, object context)
        {
            var frameContextsString = await frame.EvaluateAsync<string>(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("getFrameContexts.js"), context);
            var frameContexts = JsonConvert.DeserializeObject<List<AxeFrameContext>>(frameContextsString);
            if (frameContexts == null)
            {
                return new List<AxeFrameContext>();
            }
            return frameContexts;
        }

        /// <summary>
        /// Injects scripts into the current frame and configures axe
        /// </summary>
        private async Task ConfigureAxe(IFrame frame)
        {
            await m_axeContentEmbedder.EmbedAxeCoreIntoFrame(frame);
            await frame.EvaluateAsync(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("branding.js"));
        }

        private async Task<bool> RunPartialExists(IFrame frame)
        {
            return await frame.EvaluateAsync<bool>(await EmbeddedResourceProvider.ReadEmbeddedFileAsync("runPartialExists.js"));
        }
    }
}
