#nullable enable

using System.Text.Json.Serialization;

namespace Playwright.Axe
{
    /// <summary>
    /// Axe Result Group.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AxeResultGroup
    {
        /// <summary>
        /// Rules for which no matching elements were found on the page.
        /// </summary>
        Inapplicable,

        /// <summary>
        /// Nodes that have passed.
        /// </summary>
        Passes,

        /// <summary>
        /// Nodes that could neither be determined to definitively pass or definitively fail.
        /// </summary>
        Incomplete,

        /// <summary>
        /// Keeps track of all the failed nodes.
        /// </summary>
        Violations
    }
}
