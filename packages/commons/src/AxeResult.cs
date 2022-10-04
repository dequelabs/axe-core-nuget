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
        /// The Error that was found on the application that ran the audit.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The Name of the application that ran the audit.
        /// </summary>
        public string TestEngineName { get; private set; }

        /// <summary>
        /// The Version of the application that ran the audit.
        /// </summary>
        public string TestEngineVersion { get; private set; }

        /// <summary>
        /// The tool options used for the configuration of the data format used by axe.
        /// </summary>
        public object ToolOptions { get; private set; }

        public AxeResult(JObject result)
        {
            JToken violationsToken = result.SelectToken("violations");
            JToken passesToken = result.SelectToken("passes");
            JToken inapplicableToken = result.SelectToken("inapplicable");
            JToken incompleteToken = result.SelectToken("incomplete");
            JToken timestampToken = result.SelectToken("timestamp");
            JToken urlToken = result.SelectToken("url");
            JToken testEnvironment = result.SelectToken("testEnvironment");
            JToken testRunner = result.SelectToken("testRunner");
            JToken testEngine = result.SelectToken("testEngine");
            JToken testEngineName = testEngine?.SelectToken("name");
            JToken testEngineVersion = testEngine?.SelectToken("version");
            JToken toolOptions = result?.SelectToken("toolOptions");
            JToken error = result.SelectToken("error");

            Violations = violationsToken?.ToObject<AxeResultItem[]>();
            Passes = passesToken?.ToObject<AxeResultItem[]>();
            Inapplicable = inapplicableToken?.ToObject<AxeResultItem[]>();
            Incomplete = incompleteToken?.ToObject<AxeResultItem[]>();
            Timestamp = timestampToken?.ToObject<DateTimeOffset>();
            TestEnvironment = testEnvironment?.ToObject<AxeTestEnvironment>();
            TestRunner = testRunner?.ToObject<AxeTestRunner>();
            Url = urlToken?.ToObject<string>();
            Error = error?.ToObject<string>();
            TestEngineName = testEngineName?.ToObject<string>();
            TestEngineVersion = testEngineVersion?.ToObject<string>();
            ToolOptions = toolOptions?.ToObject<object>();
        }
    }
}
