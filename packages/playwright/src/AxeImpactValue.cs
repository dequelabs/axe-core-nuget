#nullable enable

using System.Text.Json.Serialization;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Impact Values
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AxeImpactValue
    {
        /// <summary>
        /// Minor impact value.
        /// </summary>
        Minor,

        /// <summary>
        /// Moderate impact value.
        /// </summary>
        Moderate,

        /// <summary>
        /// Serious impact value.
        /// </summary>
        Serious,

        /// <summary>
        /// Critical impact value.
        /// </summary>
        Critical
    }
}
