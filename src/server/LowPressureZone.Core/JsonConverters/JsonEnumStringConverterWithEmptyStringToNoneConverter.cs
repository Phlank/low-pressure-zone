using System.Text.Json;
using System.Text.Json.Serialization;

namespace LowPressureZone.Core.JsonConverters;

public sealed class JsonEnumStringConverterWithEmptyStringToNoneConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static JsonSerializerOptions StringEnumOptions => new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return GetNoneValue(typeToConvert);
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string token for {typeToConvert.Name}.");
        }

        var raw = reader.GetString();
        if (string.IsNullOrEmpty(raw))
        {
            return GetNoneValue(typeToConvert);
        }

        return JsonSerializer.Deserialize<TEnum>(ref reader, StringEnumOptions);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (EqualityComparer<TEnum>.Default.Equals(value, GetNoneValue(typeof(TEnum))))
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        JsonSerializer.Serialize(writer, value, StringEnumOptions);
    }

    private static TEnum GetNoneValue(Type typeToConvert)
    {
        if (Enum.TryParse("None", out TEnum noneValue))
        {
            return noneValue;
        }

        throw new JsonException($"{typeToConvert.Name} must define a None value to use {nameof(JsonEnumStringConverterWithEmptyStringToNoneConverter<TEnum>)}.");
    }
}

