namespace Common.utils.security;

public static class CookieUtils
{
 public const string OAUTH2_AUTHORIZATION_REQUEST_COOKIE_NAME="oauth2_auth_request";
 public const string REDIRECT_URI_PARAM_COOKIE_NAME="redirect_uri";
 public static string? GetCookie(Microsoft.AspNetCore.Http.HttpRequest request,string name) => request.Cookies.TryGetValue(name,out var value)?value:null;
 public static string? GetCookieFromResponse(Microsoft.AspNetCore.Http.HttpResponse response,string name) {
  foreach(var cookie in Microsoft.Net.Http.Headers.SetCookieHeaderValue.ParseList(response.Headers.SetCookie.Where(v => v is not null).Select(v => v!).ToArray()))
   if(string.Equals(cookie.Name.Value,name,StringComparison.Ordinal))return cookie.Value.Value;
  return null;
 }
 public static void AddCookie(Microsoft.AspNetCore.Http.HttpResponse response,string name,string value,int maxAge,bool isSecure,bool httpOnly) =>
  response.Cookies.Append(name,value,new Microsoft.AspNetCore.Http.CookieOptions{Path="/",HttpOnly=httpOnly,Secure=isSecure,MaxAge=maxAge<0?null:TimeSpan.FromSeconds(maxAge)});
 public static void AddCookie(Microsoft.AspNetCore.Http.HttpResponse response,string name,string domain,string value,int maxAge,bool isSecure,bool httpOnly) =>
  response.Cookies.Append(name,value,new Microsoft.AspNetCore.Http.CookieOptions{Path="/",Domain=domain,HttpOnly=httpOnly,Secure=isSecure,MaxAge=maxAge<0?null:TimeSpan.FromSeconds(maxAge)});
 public static Microsoft.Net.Http.Headers.SetCookieHeaderValue CreateCookie(string name,string value,long maxAge,bool isSecure) =>
  new(name,value){Path="/",MaxAge=TimeSpan.FromSeconds(maxAge),SameSite=Microsoft.Net.Http.Headers.SameSiteMode.None,HttpOnly=true,Secure=isSecure};
 public static Microsoft.Net.Http.Headers.SetCookieHeaderValue CreateCookie(string name,string value,string path,long maxAge) =>
  new(name,value){Path=path,MaxAge=TimeSpan.FromSeconds(maxAge),SameSite=Microsoft.Net.Http.Headers.SameSiteMode.None,HttpOnly=true};
 public static void DeleteCookie(Microsoft.AspNetCore.Http.HttpRequest request,Microsoft.AspNetCore.Http.HttpResponse response,string name) {
  if(request.Cookies.ContainsKey(name))response.Cookies.Append(name,"",new Microsoft.AspNetCore.Http.CookieOptions{Path="/",MaxAge=TimeSpan.Zero});
 }
 [Obsolete("Prefer response.Cookies.Delete with matching domain/path options.")]
 public static Microsoft.Net.Http.Headers.SetCookieHeaderValue DeleteCookie(string name) => new(name,""){Path="/",HttpOnly=true,MaxAge=TimeSpan.Zero};
 // JSON is deliberately not Java binary serialization. Do not use for unsigned authentication state.
 public static string Serialize<T>(T value) => Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value));
 public static T? Deserialize<T>(string cookieValue) => System.Text.Json.JsonSerializer.Deserialize<T>(Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(cookieValue));
}
