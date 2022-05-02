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
        Inapplicable,
        Passes,
        Incomplete,
        Violations
    }
}
