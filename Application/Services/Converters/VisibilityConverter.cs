using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain;

namespace Application.Services.Converters
{
    public class VisibilityConverter : JsonConverter<Visibility?>
    {
        public override Visibility? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();
                if (value == null)
                {
                    return null;
                }

                if (Enum.TryParse<Visibility>(value, true, out var visibility))
                {
                    return visibility;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Visibility? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}