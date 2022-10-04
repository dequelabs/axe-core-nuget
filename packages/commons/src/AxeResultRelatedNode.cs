using Newtonsoft.Json;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Information about other nodes related to a check.
    /// </summary>
    public class AxeResultRelatedNode
    {
        /// <summary>
        /// HTML source of the related node.
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// List of selectors for the related node.
        /// </summary>
        [JsonProperty("target", ItemConverterType = typeof(AxeResultTargetConverter), NullValueHandling = NullValueHandling.Ignore)]
        public List<AxeResultTarget> Target { get; set; }
    }
}
