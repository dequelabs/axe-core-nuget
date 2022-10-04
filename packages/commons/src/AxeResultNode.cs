using Newtonsoft.Json;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// A tested rule element.
    /// </summary>
    public class AxeResultNode
    {
        /// <summary>
        /// Target identifier of this result node.
        /// </summary>
        [JsonProperty("target", ItemConverterType = typeof(AxeResultTargetConverter), NullValueHandling = NullValueHandling.Ignore)]
        public List<AxeResultTarget> Target { get; set; }

        /// <summary>
        /// Xpath selectors for elements
        /// </summary>
        public List<string> XPath { get; set; }

        /// <summary>
        /// Elements ancestry.
        /// </summary>
        public List<string> Ancestry { get; set; }

        /// <summary>
        /// Snippet of HTML of the Element.
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// How serious the violation is.
        /// </summary>
        public string Impact { get; set; }

        /// <summary>
        /// List of checks that were made where at least one must have passed.
        /// </summary>
        public AxeResultCheck[] Any { get; set; }

        /// <summary>
        /// List of checks that were made where all must have passed.
        /// </summary>
        public AxeResultCheck[] All { get; set; }

        /// <summary>
        /// List of checks that were made where all must have not passed.
        /// </summary>
        public AxeResultCheck[] None { get; set; }
    }
}
