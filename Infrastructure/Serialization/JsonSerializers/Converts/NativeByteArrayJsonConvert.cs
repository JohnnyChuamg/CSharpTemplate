using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serialization.JsonSerializers.Converts;

public class NativeByteArrayJsonConvert : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var memoryStream = new MemoryStream();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.StartArray) continue;
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new FormatException();
            }

            if (!reader.TryGetByte(out var result))
            {
                throw new FormatException();
            }

            memoryStream.WriteByte(result);
        }

        return memoryStream.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, byte[]? data, JsonSerializerOptions options)
    {
        if (data == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var value in data)
        {
            writer.WriteNumberValue((uint)value);
        }

        writer.WriteEndArray();
    }
}