using Newtonsoft.Json;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Has the list of selectors that have to be included or excluded from scanning.
    /// If not specified the whole document will be scanned.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AxeRunContext
    {
        /// <summary>
        /// List of <cref see="AxeSelector"/>s to include in the scan. 
        /// If not specified it includes the entire document by default.
        /// </summary>
        [JsonProperty("include")]
        public List<AxeSelector> Include { get; set; }

        /// <summary>
        /// List of <cref see="AxeSelector"/>s to exclude from the scan.
        /// Overrides <cref see="Include"/> if both are specified.
        /// </summary>
        [JsonProperty("exclude")]
        public List<AxeSelector> Exclude { get; set; }
    }
}
