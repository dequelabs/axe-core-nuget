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
        [JsonProperty("include")]
        public List<AxeSelector> Include { get; set; }
        [JsonProperty("exclude")]
        public List<AxeSelector> Exclude { get; set; }
    }
}
