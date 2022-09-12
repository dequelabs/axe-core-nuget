using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deque.AxeCore.Commons
{
    class AxeSelectorJsonConverter : JsonConverter<AxeSelector>
    {
        public override AxeSelector ReadJson(JsonReader reader, Type objectType, AxeSelector existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Date)
            {
                return new AxeSelector(serializer.Deserialize<string>(reader));
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException($"Cannot deserialize AxeSelector: expected a string or array, but found token type ${reader.TokenType}");
            }

            var rawFrameShadowSelectors = serializer.Deserialize<List<object>>(reader);
            return AxeSelector.FromFrameShadowSelectors(rawFrameShadowSelectors.Select(rawShadowSelectors =>
            {
                string singleSelector = rawShadowSelectors as string;
                if (singleSelector != null)
                {
                    return new List<string> { singleSelector };
                }

                JArray multipleSelectors = rawShadowSelectors as JArray;
                if (multipleSelectors != null)
                {
                    return multipleSelectors.Select(token =>
                    {
                        try
                        {
                            return (string)token;
                        }
                        catch (InvalidCastException)
                        {
                            throw new JsonSerializationException($"Cannot deserialize AxeSelector: expected array-of-array elements to be strings, but found a non-string token {token}");
                        }
                    }).ToList();
                }

                throw new JsonSerializationException($"Cannot deserialize AxeSelector: expected array elements to be either strings or arrays of strings, but found {rawShadowSelectors}");
            }).ToList());
        }

        public override void WriteJson(JsonWriter writer, AxeSelector value, JsonSerializer serializer)
        {
            if (value.FrameShadowSelectors.Count == 1 && value.FrameShadowSelectors[0].Count == 1)
            {
                serializer.Serialize(writer, value.FrameShadowSelectors[0][0]);
                return;
            }

            serializer.Serialize(writer, value.FrameShadowSelectors.Select<List<string>, object /* either List<string> or string */>(shadowSelectors =>
            {
                if (shadowSelectors.Count == 1)
                {
                    return shadowSelectors[0];
                }
                else
                {
                    return shadowSelectors;
                }
            }));
        }
    }
}
