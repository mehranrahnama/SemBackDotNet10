namespace Common.utils.deserializer;

public sealed class OptionalBigDecimalDeserializer : System.Text.Json.Serialization.JsonConverter<decimal?>
{
 public override bool HandleNull => true;
 public override decimal? Read(ref System.Text.Json.Utf8JsonReader reader,Type typeToConvert,System.Text.Json.JsonSerializerOptions options) {
  if(reader.TokenType==System.Text.Json.JsonTokenType.Null)return null;
  string? raw;
  if(reader.TokenType==System.Text.Json.JsonTokenType.String)raw=reader.GetString();
  else if(reader.TokenType==System.Text.Json.JsonTokenType.Number) {using var doc=System.Text.Json.JsonDocument.ParseValue(ref reader);raw=doc.RootElement.GetRawText();}
  else throw new System.Text.Json.JsonException("Expected decimal or string");
  if(string.IsNullOrWhiteSpace(raw))return null;
  if(decimal.TryParse(raw.Trim(),System.Globalization.NumberStyles.AllowLeadingSign|System.Globalization.NumberStyles.AllowDecimalPoint|System.Globalization.NumberStyles.AllowExponent,System.Globalization.CultureInfo.InvariantCulture,out var value))return value;
  throw new global::SemBackDotNet10.Common.exception.BusinessException("مقدار وارد شده معتبر نیست.");
 }
 public override void Write(System.Text.Json.Utf8JsonWriter writer,decimal? value,System.Text.Json.JsonSerializerOptions options) {
  if(value is null)writer.WriteNullValue();else writer.WriteNumberValue(value.Value);
 }
}
