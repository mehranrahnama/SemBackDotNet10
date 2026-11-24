namespace Common.utils.deserializer;

public sealed class OptionalLongDeserializer : System.Text.Json.Serialization.JsonConverter<long?>
{
 public override bool HandleNull => true;
 public override long? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options) {
  if(reader.TokenType==System.Text.Json.JsonTokenType.Null)return null;
  string? raw=reader.TokenType switch {
   System.Text.Json.JsonTokenType.String => reader.GetString(),
   System.Text.Json.JsonTokenType.Number => ReadNumber(ref reader),
   _ => throw new System.Text.Json.JsonException("Expected integer or string")
  };
  if(string.IsNullOrWhiteSpace(raw))return null;
  if(long.TryParse(raw.Trim(),System.Globalization.NumberStyles.AllowLeadingSign,System.Globalization.CultureInfo.InvariantCulture,out var value))return value;
  throw new System.Text.Json.JsonException("مقدار وارد شده باید عدد صحیح باشد: "+raw);
 }
 private static string ReadNumber(ref System.Text.Json.Utf8JsonReader reader) { using var doc=System.Text.Json.JsonDocument.ParseValue(ref reader);return doc.RootElement.GetRawText(); }
 public override void Write(System.Text.Json.Utf8JsonWriter writer,long? value,System.Text.Json.JsonSerializerOptions options) {
  if(value is null)writer.WriteNullValue();else writer.WriteNumberValue(value.Value);
 }
}
