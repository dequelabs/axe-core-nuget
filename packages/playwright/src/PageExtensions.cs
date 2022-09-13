#nullable enable

using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright.AxeContent;
using Deque.AxeCore.Playwright.AxeCoreWrapper;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Extensions for adding Axe to Playwright Page.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Retrieves rule known in Axe.
        /// </summary>
        /// <param name="page">The Playwright Page object.</param>
        /// <param name="tags">Array of tags used to filter returned rules.</param>
        /// <returns>List of <see cref="AxeRuleMetadata"/></returns>
        public static async Task<IList<AxeRuleMetadata>> GetAxeRules(this IPage page, IList<string>? tags = null)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            return await axeCoreWrapper.GetRules(page, tags);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(this IPage page, AxeRunOptions? options = null)
        {
            if (runLagacy) {
                return aRunAxeLegacy(page, null, options);
            }
            var partialResults = new List<PartialResult>();

            var frameContexts = GetFrameContexts(context);

            try
            {
                var topResultString = (string) ExecuteAsyncScript(
                                                                  AxeScripts.AxeRunPartialCommand,
                                                                  context,
                                                                  runOptions
                );
                partialResults.Add(JsonConverter.Deserialize<PartialResult>(topResultString));
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Error executing runPartial. Error message: {e.ToString()}");
                throw e;
            }

            frameContexts.ForEach(frameContext =>
                                  {
                                  partialResults.AddRange(RunPartialRecursive(
                                                                              frameContext,
                                                                              runOptions
                                  ));
                                  });

            // isolate finishRun
            return IsolatedFinishRun(partialResults.ToArray(), runOptions);
        }

        /// <summary>
        /// Runs Axe against the page in its current state.
        /// </summary>
        /// <param name="page">The Playwright Page object</param>
        /// <param name="context">Context to specify which element to run axe on.</param>
        /// <param name="options">Options for running Axe.</param>
        /// <returns>The AxeResults</returns>
        public static async Task<AxeResults> RunAxe(
                                                    this IPage page,
                                                    AxeRunContext context,
                                                    AxeRunOptions? options = null)
        {
            if (runLagacy) {
                return aRunAxeLegacy(page, context, options);
            }

            var partialResults = new List<PartialResult>();

            var frameContexts = GetFrameContexts(page, context);

            try
            {
                var topResultString = (string) ExecuteAsyncScript(
                                                                  page,
                                                                  AxeScripts.AxeRunPartialCommand,
                                                                  context,
                                                                  runOptions
                );
                partialResults.Add(JsonConverter.Deserialize<PartialResult>(topResultString));
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Error executing runPartial. Error message: {e.ToString()}");
                throw e;
            }

            frameContexts.ForEach(frameContext =>
                                  {
                                  partialResults.AddRange(RunPartialRecursive(
                                                                              page,
                                                                              frameContext,
                                                                              runOptions
                                  ));
                                  });

            // isolate finishRun
            return IsolatedFinishRun(partialResults.ToArray(), runOptions);
        }

        private static async Task<AxeResults> RunAxeLegacy(this IPage page, AxeRunContext? context, AxeRunOptions? options)
        {
            IAxeScriptProvider axeScriptProvider = new BundledAxeScriptProvider();
            IAxeContentEmbedder axeContentEmbedder = new DefaultAxeContentEmbedder(axeScriptProvider);

            IAxeCoreWrapper axeCoreWrapper = new DefaultAxeCoreWrapper(axeContentEmbedder);

            AxeResults results = await axeCoreWrapper.Run(page, context, options);
            IFileSystem fileSystem = new FileSystem();

            return results;
        }

        private List<PartialResult> RunPartialRecursive(
                                                        this IPage page,
                                                        FrameContext frameContext,
                                                        RunOptions.RunOptions runOptions
        )
        {
            var partialResults = new List<PartialResult>();

            try
            {
                // get the proper selector the frame and switch to it
                var selector = ExecuteScript(page, AxeScripts.AxeShadowSelectCommand, frameContext.Selector);
                webDriver.SwitchTo().Frame(selector as IWebElement);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Unable to switch to iframe. Error message: {ex.ToString()}");
                webDriver.SwitchTo().ParentFrame();

                partialResults.Add(null);

                return partialResults;
            }

            // inject axe and configure it
            ConfigureAxe();

            try
            {
                var frameResultSring = (string) ExecuteAsyncScript(
                                                                   page,
                                                                   AxeScripts.AxeRunPartialCommand,
                                                                   frameContext.Context,
                                                                   runOptions
                );
                partialResults.Add(JsonConverter.Deserialize<PartialResult>(frameResultSring));
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Error executing runPartial. Error message: {e.ToString()}");
                webDriver.SwitchTo().ParentFrame();

                partialResults.Add(null);

                return partialResults;
            }

            var frameContexts = GetFrameContexts(frameContext.Context);

            frameContexts.ForEach(fContext =>
                                  {
                                  partialResults.AddRange(RunPartialRecursive(
                                                                              page,
                                                                              fContext,
                                                                              runOptions
                                  ));
                                  });

            webDriver.SwitchTo().ParentFrame();

            return partialResults;
        }

        private TestResults IsolatedFinishRun(PartialResult[] partialResults, RunOptions.RunOptions options)
        {
            // grab reference to current window
            var originalWindowHandle = webDriver.CurrentWindowHandle;

            try
            {
                // create an isolated page
                ExecuteScript("window.open('about:blank', '_blank')");
                webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
                webDriver.Navigate().GoToUrl("about:blank");
            }
            catch (Exception e)
            {
                throw new AxeSeleniumException(
                                               $"Failed to switch windows. Please make sure you have popup blockers disabled and you are using the correct browser drivers.{Environment.NewLine}Please check out https://axe-devtools-html-docs.deque.com/reference/csharp/error-handling.html",
                                               e
                );
            }

            ConfigureAxe();

            // grab result ...
            var result = (string) ExecuteAsyncScript(AxeScripts.AxeFinishRunCommand, partialResults, options);

            // ... close the new window and go back
            webDriver.Close();
            webDriver.SwitchTo().Window(originalWindowHandle);

            return JsonConverter.Deserialize<TestResults>(result);
        }
    }
}
