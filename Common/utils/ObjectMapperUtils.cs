namespace Common.utils;

public static class ObjectMapperUtils
{
 public static string ConvertToUrlEncoded(object obj) {
  ArgumentNullException.ThrowIfNull(obj);
  using var doc=System.Text.Json.JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(obj));
  if(doc.RootElement.ValueKind!=System.Text.Json.JsonValueKind.Object)throw new ArgumentException("Expected an object",nameof(obj));
  return string.Join("&",doc.RootElement.EnumerateObject().Select(p=> {
   string value=p.Value.ValueKind switch {
    System.Text.Json.JsonValueKind.String => p.Value.GetString() ?? "",
    System.Text.Json.JsonValueKind.Number => p.Value.GetRawText(),
    System.Text.Json.JsonValueKind.True => "true",System.Text.Json.JsonValueKind.False => "false",_ => ""
   };
   return System.Net.WebUtility.UrlEncode(p.Name)+"="+System.Net.WebUtility.UrlEncode(value);
  }));
 }
}
