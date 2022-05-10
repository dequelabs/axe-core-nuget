#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playwright.Axe
{
    /// <summary>
    /// Converter for Run Context
    /// </summary>
    internal sealed class RunContextJsonConverter : JsonConverter<AxeRunContext>
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public override AxeRunContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization not supported");
        }

        public override void Write(Utf8JsonWriter writer, AxeRunContext value, JsonSerializerOptions options)
        {
            if(value is AxeRunSerialContext serialContext)
            {
                writer.WriteRawValue(JsonSerializer.Serialize(serialContext, s_jsonOptions));
            }
            else
            {
                throw new NotImplementedException("Context type not recognized");
            }
        }
    }
}
