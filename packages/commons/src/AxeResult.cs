using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Represents the complete results of an axe scan, including results from all rules and nodes included in the scan.
    /// </summary>
    public class AxeResult
    {
        /// <summary>
        /// These results indicate what elements failed the rules.
        /// </summary>
        public AxeResultItem[] Violations { get; }

        /// <summary>
        /// These results indicate what elements passed the rules.
        /// </summary>
        public AxeResultItem[] Passes { get; }

        /// <summary>
        /// These results indicate which rules did not run because no matching content was found on the page. 
        /// For example, with no video, those rules won't run.
        /// </summary>
        public AxeResultItem[] Inapplicable { get; }

        /// <summary>
        /// These results were aborted and require further testing. 
        /// This can happen either because of technical restrictions to what the rule can test, or because a javascript error occurred.
        /// </summary>
        public AxeResultItem[] Incomplete { get; }

        /// <summary>
        /// The date and time that analysis was completed.
        /// </summary>
        public DateTimeOffset? Timestamp { get; private set; }

        /// <summary>
        /// Information about the current browser or node application that ran the audit.
        /// </summary>
        public AxeTestEnvironment TestEnvironment { get; private set; }

        /// <summary>
        /// The runner that ran the audit.
        /// </summary>
        public AxeTestRunner TestRunner { get; set; }

        /// <summary>
        /// The URL of the page that was tested.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The application that ran the audit.
        /// </summary>
        public AxeTestEngine TestEngine { get; private set; }

        /// <summary>
        /// The tool options used for the configuration of the data format used by axe.
        /// </summary>
        public object ToolOptions { get; private set; }

        public AxeResult(JObject result)
        {
            // Some (but not all) WebDrivers treat objects with an error property as a JavaScript error
            // and don't reach this point, but for those that don't, we handle it as an error ourselves.
            string error = result.SelectToken("error")?.ToObject<string>();
            if (error != null)
            {
                throw new Exception($"JavaScript error occurred while running axe-core in page: {error}");
            }

            JToken violationsToken = result.SelectToken("violations");
            JToken passesToken = result.SelectToken("passes");
            JToken inapplicableToken = result.SelectToken("inapplicable");
            JToken incompleteToken = result.SelectToken("incomplete");
            JToken timestampToken = result.SelectToken("timestamp");
            JToken urlToken = result.SelectToken("url");
            JToken testEnvironment = result.SelectToken("testEnvironment");
            JToken testRunner = result.SelectToken("testRunner");
            JToken testEngine = result.SelectToken("testEngine");

            JToken toolOptions = result?.SelectToken("toolOptions");

            Violations = violationsToken?.ToObject<AxeResultItem[]>();
            Passes = passesToken?.ToObject<AxeResultItem[]>();
            Inapplicable = inapplicableToken?.ToObject<AxeResultItem[]>();
            Incomplete = incompleteToken?.ToObject<AxeResultItem[]>();
            Timestamp = timestampToken?.ToObject<DateTimeOffset>();
            TestEnvironment = testEnvironment?.ToObject<AxeTestEnvironment>();
            TestRunner = testRunner?.ToObject<AxeTestRunner>();
            Url = urlToken?.ToObject<string>();
            TestEngine = testEngine?.ToObject<AxeTestEngine>();
            ToolOptions = toolOptions?.ToObject<object>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, AxeJsonSerializerSettings.WithFormatting(Formatting.Indented));
        }
    }
}
