using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Deque.AxeCore.Commons;

namespace Deque.AxeCore.Selenium
{
    /// <summary>
    /// Fluent style builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
    /// exclude(), and options() methods before calling analyze() to run.
    /// </summary>
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly AxeBuilderOptions _AxeBuilderOptions;
        private readonly AxeRunContext runContext = new AxeRunContext();
        private AxeRunOptions runOptions = new AxeRunOptions();
        private string outputFilePath = null;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include
        };

        /// <summary>
        /// The run options to be passed to axe. Refer https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter
        /// Cannot not be used with <see cref="WithRules(string[])"/>, <see cref="WithTags(string[])"/> & <see cref="DisableRules(string[])"/>
        /// </summary>
        [Obsolete("Use WithOptions / WithTags / WithRules / DisableRules apis")]
        public string Options { get; set; } = "{}";

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        public AxeBuilder(IWebDriver webDriver) : this(webDriver, new AxeBuilderOptions { ScriptProvider = new BundledAxeScriptProvider() })
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        /// <param name="options">Builder options</param>
        public AxeBuilder(IWebDriver webDriver, AxeBuilderOptions options)
        {
            ValidateNotNullParameter(webDriver, nameof(webDriver));
            ValidateNotNullParameter(options, nameof(options));

            _webDriver = webDriver;
            _AxeBuilderOptions = options;
        }

        /// <summary>
        ///  Run configuration data that is passed to axe for scanning the web page.
        ///  This will override the value set by <see cref="WithRules(string[])"/>, <see cref="WithTags(string[])"/> & <see cref="DisableRules(string[])"/>
        /// </summary>
        /// <param name="runOptions">run options to be used for scanning. </param>
        public AxeBuilder WithOptions(AxeRunOptions runOptions)
        {
            ValidateNotNullParameter(runOptions, nameof(runOptions));

            ThrowIfDeprecatedOptionsSet();

            this.runOptions = runOptions;

            return this;
        }

        /// <summary>
        /// Limit analysis to only the specified tags.
        /// Refer https://www.deque.com/axe/axe-for-web/documentation/api-documentation/#api-name-axegetrules to get the list of supported tag names
        /// Cannot be used with <see cref="WithRules(string[])"/> & <see cref="Options"/>
        /// </summary>
        /// <param name="tags">tags to be used for scanning</param>
        public AxeBuilder WithTags(params string[] tags)
        {
            ValidateParameters(tags, nameof(tags));

            ThrowIfDeprecatedOptionsSet();

            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = tags.ToList()
            };
            return this;
        }

        /// <summary>
        /// Limit analysis to only the specified rules.
        /// Refer https://dequeuniversity.com/rules/axe/ to get the complete listing of available rule IDs.
        /// Cannot be used with <see cref="WithTags(string[])"/> & <see cref="Options"/>
        /// </summary>
        /// <param name="rules">rule IDs to be used for scanning</param>
        public AxeBuilder WithRules(params string[] rules)
        {
            ValidateParameters(rules, nameof(rules));

            ThrowIfDeprecatedOptionsSet();

            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "rule",
                Values = rules.ToList()
            };

            return this;
        }

        /// <summary>
        ///  Set the list of rules to skip when running an analysis
        ///  Refer https://dequeuniversity.com/rules/axe/ to get the complete listing of available rule IDs.
        ///  Cannot be used with <see cref="Options"/>
        /// </summary>
        /// <param name="rules">rule IDs to be skipped from analysis</param>
        public AxeBuilder DisableRules(params string[] rules)
        {
            ValidateParameters(rules, nameof(rules));

            ThrowIfDeprecatedOptionsSet();

            var rulesMap = new Dictionary<string, RuleOptions>();
            foreach (var rule in rules)
            {
                rulesMap[rule] = new RuleOptions
                {
                    Enabled = false
                };
            }
            runOptions.Rules = rulesMap;
            return this;
        }

        /// <summary>
        /// Restricts the analysis to include only the given CSS selector and its descendants, instead of the whole page.
        /// To include multiple selectors, call Include multiple times.
        /// To include a selector inside an iframe, see <see cref="Include(AxeSelector)"/> and <see cref="AxeSelector.AxeSelector(string, List{string})"/>.
        /// To include a selector inside a shadow DOM, see <see cref="Include(AxeSelector)"/> and <see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>.
        /// </summary>
        /// <param name="selector">A CSS selector in the topmost frame of the page</param>
        public AxeBuilder Include(string selector) => this.Include(new AxeSelector(selector));

        /// <summary>
        /// Restricts the analysis to include only the given <see cref="AxeSelector"/> and its descendants, instead of the whole page.
        /// This overload can include selectors inside iframes (with <see cref="AxeSelector.AxeSelector(string, List{string})"/>) or shadow DOMs
        /// (<see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>).
        /// To include multiple selectors, call Include multiple times.
        /// </summary>
        /// <param name="selector">An <see cref="AxeSelector"/> representing an element anywhere in the page, including nested in an iframe or shadow DOM</param>
        public AxeBuilder Include(AxeSelector selector)
        {
            ValidateNotNullParameter(selector, nameof(selector));

            runContext.Include = runContext.Include ?? new List<AxeSelector>();
            runContext.Include.Add(selector);
            return this;
        }

        /// <summary>
        /// Excludes the given CSS selector and its descendants from analysis.
        /// To exclude multiple selectors, call Exclude multiple times.
        /// To exclude a selector inside an iframe, see <see cref="Exclude(AxeSelector)"/> and <see cref="AxeSelector.AxeSelector(string, List{string})"/>.
        /// To exclude a selector inside a shadow DOM, see <see cref="Exclude(AxeSelector)"/> and <see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>.
        /// </summary>
        /// <param name="selector">A CSS selector in the topmost frame of the page</param>
        public AxeBuilder Exclude(string selector) => this.Exclude(new AxeSelector(selector));

        /// <summary>
        /// Excludes the given <see cref="AxeSelector"/> and its descendants from analysis. This overload can exclude selectors inside iframes
        /// (with <see cref="AxeSelector.AxeSelector(string, List{string})"/>) or shadow DOMs (<see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>).
        /// To exclude multiple selectors, call Exclude multiple times.
        /// </summary>
        /// <param name="selector">An AxeSelector representing an element anywhere in the page, including nested in an iframe or shadow DOM</param>
        public AxeBuilder Exclude(AxeSelector selector)
        {
            ValidateNotNullParameter(selector, nameof(selector));

            runContext.Exclude = runContext.Exclude ?? new List<AxeSelector>();
            runContext.Exclude.Add(selector);
            return this;
        }

        /// <summary>
        /// Causes <see cref="Analyze()"/> to write the axe results as a JSON file, in addition to returning it in object format as usual.
        /// File will be overwritten if already exists.
        /// </summary>
        /// <param name="path">Path to the output file. Will be passed as-is to the System.IO APIs.</param>
        public AxeBuilder WithOutputFile(string path)
        {
            ValidateNotNullParameter(path, nameof(path));

            outputFilePath = path;
            return this;
        }

        /// <summary>
        /// Run axe against a specific WebElement (including its descendants).
        /// </summary>
        /// <param name="context">A WebElement to test</param>
        /// <returns>An axe results document</returns>
        public AxeResult Analyze(IWebElement context)
        {
            return AnalyzeRawContext(context);
        }

        /// <summary>
        /// Run axe against the entire page.
        /// </summary>
        /// <returns>An axe results document</returns>
        public AxeResult Analyze()
        {
            bool runContextHasData = runContext.Include?.Any() == true || runContext.Exclude?.Any() == true;

            string rawContext = runContextHasData ? JsonConvert.SerializeObject(runContext, JsonSerializerSettings) : null;

            return AnalyzeRawContext(rawContext);
        }

        /// <summary>
        /// Runs axe via scan.js at a specific context, which will be passed as-is to Selenium for scan.js to interpret, and
        /// parses/handles the scan.js output per the current builder options.
        /// </summary>
        /// <param name="rawContextArg">The value to pass as-is to scan.js to use as the axe.run "context" argument</param>
        private AxeResult AnalyzeRawContext(object rawContextArg)
        {
            _webDriver.Inject(_AxeBuilderOptions.ScriptProvider, runOptions);

#pragma warning disable CS0618 // Intentionally falling back to publicly deprecated property for backcompat
            string rawOptionsArg = Options == "{}" ? JsonConvert.SerializeObject(runOptions, JsonSerializerSettings) : Options;
#pragma warning restore CS0618

            string scanJsContent = EmbeddedResourceProvider.ReadEmbeddedFile("scan.js");
            object[] rawArgs = new[] { rawContextArg, rawOptionsArg };
            var result = ((IJavaScriptExecutor)_webDriver).ExecuteAsyncScript(scanJsContent, rawArgs);

            JObject jObject = JObject.FromObject(result);

            if (outputFilePath != null && jObject.Type == JTokenType.Object)
            {
                Encoding utf8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                using (var outputFileWriter = new StreamWriter(outputFilePath, append: false, encoding: utf8NoBOM))
                {
                    jObject.WriteTo(new JsonTextWriter(outputFileWriter));
                }
            }

            return new AxeResult(jObject);
        }

        private static void ValidateParameters(string[] parameterValue, string parameterName)
        {
            ValidateNotNullParameter(parameterValue, parameterName);

            if (parameterValue.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("There is some items null or empty", parameterName);
            }
        }

        private static void ValidateNotNullParameter<T>(T parameterValue, string parameterName)
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        private void ThrowIfDeprecatedOptionsSet()
        {
#pragma warning disable CS0618 // Intentionally checking publicly deprecated property for backcompat
            if (Options != "{}")
#pragma warning restore CS0618
            {
                throw new InvalidOperationException("Deprecated Options api shouldn't be used with the new apis - WithOptions/WithRules/WithTags or DisableRules");
            }
        }
    }
}
