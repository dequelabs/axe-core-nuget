using Newtonsoft.Json.Serialization;
using System;

namespace Deque.AxeCore.Commons
{
    // A converter registered in JsonSerializerSettings.Converters does not override the class-level
    // [JsonConverter] attribute on AxeSelector; assigning the contract's converter here does.
    internal class AxeSelectorContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (objectType == typeof(AxeSelector))
            {
                contract.Converter = new AxeSelectorJsonConverter(arraySelectors: true);
            }

            return contract;
        }
    }
}
