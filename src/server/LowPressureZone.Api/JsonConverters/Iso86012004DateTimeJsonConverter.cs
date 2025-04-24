using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LowPressureZone.Api.JsonConverters;

public class Iso86012004DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Cannot parse value. Reason: Not a string");

        var array = new char[24];
        var token = array.AsSpan();
        reader.CopyString(token);
        // ISO 8601-2004 format
        // YYYY-MM-DDTHH:mm:SS+0000
        // Year slice: 0, 4
        // Month slice: 5, 2
        // Day slice: 8, 2
        // Hour slice: 11, 2
        // Minute slice: 14, 2
        // Second slice: 17, 2
        return new DateTime(ushort.Parse(token.Slice(0, 4), NumberStyles.None, CultureInfo.InvariantCulture),
                            ushort.Parse(token.Slice(5, 2), NumberStyles.None, CultureInfo.InvariantCulture),
                            ushort.Parse(token.Slice(8, 2), NumberStyles.None, CultureInfo.InvariantCulture),
                            ushort.Parse(token.Slice(11, 2), NumberStyles.None, CultureInfo.InvariantCulture),
                            ushort.Parse(token.Slice(14, 2), NumberStyles.None, CultureInfo.InvariantCulture),
                            ushort.Parse(token.Slice(17, 2), NumberStyles.None, CultureInfo.InvariantCulture),
                            0,
                            Calendar.CurrentEra,
                            DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}
