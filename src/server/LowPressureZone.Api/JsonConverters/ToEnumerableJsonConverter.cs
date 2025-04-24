using System.Text.Json;
using System.Text.Json.Serialization;

namespace LowPressureZone.Api.JsonConverters;

public class ToEnumerableJsonConverter<T> : JsonConverter<IEnumerable<T>>
{
    public override IEnumerable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
            return JsonSerializer.Deserialize<IEnumerable<T>>(ref reader, options)!;
        if (reader.TokenType == JsonTokenType.Null)
            return [];

        var singleItem = JsonSerializer.Deserialize<T>(ref reader, options)!;
        return [singleItem];
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}
