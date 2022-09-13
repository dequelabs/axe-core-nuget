using System;
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
        public List<string[]> Include { get; set; }
        [JsonProperty("exclude")]
        public List<string[]> Exclude { get; set; }

        /// <summary>
        /// Whether this frame was the initiator of the test
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public bool? Initiator { get; private set; }

        /// <summary>
        /// Used by axe internally
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public bool? Page { get; private set; }

        /// <summary>
        /// Whether the frame is focusable, used by axe internally
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public bool? Focusable { get; private set; }

        /// <summary>
        /// Size of the frame, used by axe internally
        /// </summary>
        /// <value></value>
        [JsonProperty]
        public object Size { get; private set; }

        /// <summary>
        /// Default constructor, initializes empty Include/Exclude arrays
        /// </summary>
        public AxeRunContext()
        {
            Include = new List<string[]>();
            Exclude = new List<string[]>();
        }

        /// <summary>
        /// Add a selector set for elements which should be tested.
        /// </summary>
        /// <param name="include">CSS selector set.</param>
        public void Including(string[] include)
        {
            Include.Add(include);
        }

        /// <summary>
        /// Add a selector set for elements which should not be tested.
        /// </summary>
        /// <param name="exclude">CSS selector set.</param>
        public void Excluding(string[] exclude)
        {
            Exclude.Add(exclude);
        }
    }
}
