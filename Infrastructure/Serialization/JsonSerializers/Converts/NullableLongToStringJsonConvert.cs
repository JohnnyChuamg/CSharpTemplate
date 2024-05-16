using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serialization.JsonSerializers.Converts;

public class NullableLongToStringJsonConvert : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                if (long.TryParse(reader.GetString(), out var result))
                {
                    return result;
                }

                break;
            }
            case JsonTokenType.Number:
            {
                if (reader.TryGetInt64(out var result))
                {
                    return result;
                }

                break;
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}