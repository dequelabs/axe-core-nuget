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
        public static JsonSerializerSettings WithFormatting(Formatting formatting)
        {
            return new JsonSerializerSettings
            {
                Formatting = formatting,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
            };
        }
    }
}
