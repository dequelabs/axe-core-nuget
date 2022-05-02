#nullable enable

using System.Text.Json.Serialization;

namespace Playwright.Axe
{
    /// <summary>
    /// Impact Values
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AxeImpactValue
    {
        Minor,
        Moderate,
        Serious,
        Critical
    }
}
