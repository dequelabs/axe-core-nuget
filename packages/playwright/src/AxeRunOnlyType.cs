#nullable enable

using System.Text.Json.Serialization;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// The type of values passed into a run only option.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AxeRunOnlyType
    {
        /// <summary>
        /// Specifies a particular rule to only run (.e.g color-contrast)
        /// </summary>
        Rule,

        /// <summary>
        /// Specifies a particular rule to only run (.e.g color-contrast)
        /// </summary>
        Rules,

        /// <summary>
        /// Specifies a particular tag to only run (.e.g wcag2aa)
        /// </summary>
        Tag,

        /// <summary>
        /// Specifies a particular tag to only run (.e.g wcag2aa)
        /// </summary>
        Tags
    }
}
