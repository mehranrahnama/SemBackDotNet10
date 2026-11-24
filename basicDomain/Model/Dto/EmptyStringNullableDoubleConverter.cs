using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public sealed class EmptyStringNullableDoubleConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDouble(out var number) && double.IsFinite(number)) return number;
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number) && double.IsFinite(number)) return number;
        }
        throw new JsonException("Expected a finite number or an empty value.");
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value);
        else writer.WriteNullValue();
    }
}
