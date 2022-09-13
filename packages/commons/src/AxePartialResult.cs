using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Results returned from axe.runPartial
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class AxePartialResult
    {
        /// <summary>
        /// List of frames tested
        /// </summary>
        /// <value></value>
        public List<SerialDqElement> Frames { get; set; }

        /// <summary>
        /// List of results from the test
        /// </summary>
        /// <value></value>
        public List<AxePartialRuleResult> Results { get; set; }

        /// <summary>
        /// Test metadata
        /// </summary>
        /// <value></value>
        public EnvironmentData EnvironmentData { get; set; }
    }

    /// <summary>
    /// Individual result returned from axe.runPartial
    /// </summary>
    public class AxePartialRuleResult
    {
        /// <summary>
        /// Rule ID
        /// </summary>
        /// <value></value>
        public string Id { get; set; }

        /// <summary>
        /// Result of the rule
        /// </summary>
        /// <value></value>
        public string Result { get; set; }

        /// <summary>
        /// Page level
        /// </summary>
        /// <value></value>
        public bool PageLevel { get; set; }

        /// <summary>
        /// Impact or severity of the rule
        /// </summary>
        /// <value></value>
        public string Impact { get; set; }

        /// <summary>
        /// Nodes affected by this rule
        /// </summary>
        /// <value></value>
        public List<Dictionary<string, object>> Nodes { get; set; }
    }

    /// <summary>
    /// Serializable frame data
    /// </summary>
    public class SerialDqElement
    {
        /// <summary>
        /// Source of the frame
        /// </summary>
        /// <value></value>
        public string Source { get; set; }

        /// <summary>
        /// Node indexes for the frame
        /// </summary>
        /// <value></value>
        public int[] NodeIndexes { get; set; }

        /// <summary>
        /// Selector for the frame
        /// </summary>
        /// <value></value>
        public object Selector { get; set; }

        /// <summary>
        /// XPath to the frame
        /// </summary>
        /// <value></value>
        public object Xpath { get; set; }

        /// <summary>
        /// Ancestry data for the frame
        /// </summary>
        /// <value></value>
        public object Ancestry { get; set; }
    }

    /// <summary>
    /// Environment metadata from the test run
    /// </summary>
    public class EnvironmentData
    {
        /// <summary>
        /// Test engine used to generate results
        /// </summary>
        /// <value></value>
        public object TestEngine { get; set; }

        /// <summary>
        /// Test runner used to generate results
        /// </summary>
        /// <value></value>
        public object TestRunner { get; set; }

        /// <summary>
        /// Test environment used to generate results
        /// </summary>
        /// <value></value>
        public object TestEnvironment { get; set; }

        /// <summary>
        /// URL of the test
        /// </summary>
        /// <value></value>
        public string Url { get; set; }

        /// <summary>
        /// Time the test was executed
        /// </summary>
        /// <value></value>
        public string Timestamp { get; set; }
    }
}
