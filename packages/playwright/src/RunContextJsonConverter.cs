#nullable enable

using Deque.AxeCore.Commons;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deque.AxeCore.Playwright
{
    /// <summary>
    /// Converter for Run Context
    /// </summary>
    internal sealed class RunContextJsonConverter : JsonConverter<AxeRunContext>
    {
        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public override AxeRunContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization not supported");
        }

        public override void Write(Utf8JsonWriter writer, AxeRunContext value, JsonSerializerOptions options)
        {
            if (value is AxeRunContext context)
            {
                writer.WriteRawValue(JsonSerializer.Serialize(context, s_jsonOptions));
            }
            else
            {
                throw new NotImplementedException("Context type not recognized");
            }
        }
    }
}
