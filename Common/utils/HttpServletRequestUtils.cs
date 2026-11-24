namespace Common.utils;

public static class HttpServletRequestUtils
{
 public static string GetCharacterEncoding(Microsoft.AspNetCore.Http.HttpRequest request) {
  if(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.TryParse(request.ContentType,out var media) && media.Charset.HasValue)
   return System.Text.Encoding.GetEncoding(media.Charset.Value!.Trim('"')).WebName;
  return System.Text.Encoding.UTF8.WebName;
 }
}
