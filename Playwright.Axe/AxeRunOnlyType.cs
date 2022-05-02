#nullable enable

using System.Text.Json.Serialization;

namespace Playwright.Axe
{
    /// <summary>
    /// The type of values passed into a run only option.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AxeRunOnlyType
    {
        Rule,
        Rules,
        Tag,
        Tags
    }
}
