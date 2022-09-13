using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// The result of axe.utils.getFrameContexts()
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class AxeFrameContext
    {
        /// <summary>
        /// DOM selector for the frame
        /// </summary>
        /// <value></value>
        [JsonProperty("frameSelector")]
        public object Selector { get; set; }

        /// <summary>
        /// Context returned for the frame
        /// </summary>
        /// <value></value>
        [JsonProperty("frameContext")]
        public AxeRunContext Context { get; set; }
    }
}
