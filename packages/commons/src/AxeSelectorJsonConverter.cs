using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deque.AxeCore.Commons
{
    // This converter is a bit complicated because axe-core can return several different kinds of JSON to represent a selector:
    //
    //   * For simple cases (no iframes, no shadow DOMs), it's just a plain string
    //   * For complex cases (involving either iframes and/or shadow DOMs), it's an array with an element per frame where each element is either:
    //       * For simple frames (no shadow DOMs), a plain string selector for the iframe/target
    //       * For complex frames (the frame/target element is in a shadow DOM), an array of selectors traversing shadow DOM root elements up to the iframe/target
    //
    // Note that a single-frame case involving a shadow DOM results in an array-of-arrays where the outer array has only one array child, as opposed to
    // the multiple-frame case involving no shadow DOMs which results in an array-of-strings.
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
            if (rawFrameShadowSelectors.Count == 0)
            {
                throw new JsonSerializationException("Cannot deserialize AxeSelector: expected a string or non-mepty array, but found empty array");
            }

            return AxeSelector.FromFrameShadowSelectors(rawFrameShadowSelectors.Select(rawShadowSelectors =>
            {
                string singleSelector = rawShadowSelectors as string;
                if (singleSelector != null)
                {
                    return new List<string> { singleSelector };
                }

                JArray multipleSelectors = rawShadowSelectors as JArray;
                if (multipleSelectors != null && multipleSelectors.Count != 0)
                {
                    return multipleSelectors.Select(token =>
                    {
                        if (token.Type == JTokenType.String || token.Type == JTokenType.Date)
                        {
                            return (string)token;
                        }

                        throw new JsonSerializationException($"Cannot deserialize AxeSelector: expected array-of-array elements to be strings, but found a non-string token {token}");
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
