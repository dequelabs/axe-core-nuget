using Newtonsoft.Json;
using System.Collections.Generic;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Has the list of selectors that have to be included or excluded from scanning. If not specified the whole document will be scanned
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AxeRunContext
    {
        /// <summary>
        /// List of items to include.
        /// If not specified it includes the entire document by default.
        /// The last string items in the array will be the CSS selector,
        /// the preceding items are specifying nested frames.
        /// </summary>
        [JsonProperty("include")]
        public List<string[]> Include { get; set; }

        /// <summary>
        /// List of items to exclude.
        /// The last string items in the array will be the CSS selector,
        /// the preceding items are specifying nested frames.
        /// </summary>
        [JsonProperty("exclude")]
        public List<string[]> Exclude { get; set; }
    }
}
