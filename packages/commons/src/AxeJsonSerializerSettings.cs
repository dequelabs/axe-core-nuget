using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Provides <see cref="Newtonsoft.Json.JsonSerializerSettings"> values intended for use with the assorted
    /// Axe* types in this namespace.
    /// </summary>
    public static class AxeJsonSerializerSettings
    {
        /// <summary>
        /// The default serialization settings recommended for use with this namespace's Axe* types.
        /// </summary>
        public static readonly JsonSerializerSettings Default = WithFormatting(Formatting.None);

        /// <summary>
        /// Produces serialization settings appropriate for use with this namespace's Axe* types with specific formatting settings.
        /// </summary>
        public static JsonSerializerSettings WithFormatting(Formatting formatting) => WithFormatting(formatting, arraySelectors: false);

        /// <summary>
        /// Produces serialization settings appropriate for use with this namespace's Axe* types with specific formatting settings.
        /// </summary>
        /// <param name="formatting">The JSON formatting to use.</param>
        /// <param name="arraySelectors">
        /// When <c>true</c>, <see cref="AxeSelector"/> values (<see cref="AxeResultNode.Target"/>, <see cref="AxeResultNode.XPath"/>
        /// and <see cref="AxeResultNode.Ancestry"/>) serialize as arrays in all cases, matching the shape axe-core itself emits.
        /// When <c>false</c> (the default), a selector which involves no iframes or shadow DOMs serializes as a bare string.
        /// </param>
        public static JsonSerializerSettings WithFormatting(Formatting formatting, bool arraySelectors)
        {
            DefaultContractResolver contractResolver = arraySelectors
                ? new AxeSelectorContractResolver()
                : new DefaultContractResolver();
            contractResolver.NamingStrategy = new CamelCaseNamingStrategy();

            return new JsonSerializerSettings
            {
                Formatting = formatting,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = contractResolver,
            };
        }
    }
}
