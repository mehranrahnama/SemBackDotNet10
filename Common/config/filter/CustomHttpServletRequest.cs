namespace Common.config.filter;

public sealed class CustomHttpServletRequest(Microsoft.AspNetCore.Http.HttpRequest request)
{
 private readonly Dictionary<string,string> customParameters = new(StringComparer.Ordinal);
 public void AddHeader(string name,string value) => customParameters[name]=value;
 // Despite its Java name, AddHeader overrides parameters, not HTTP headers.
 public string? GetParameter(string name) {
  if(customParameters.TryGetValue(name,out var value))return value;
  if(request.Query.TryGetValue(name,out var query))return query.Count>0?query[0]:null;
  if(request.HasFormContentType && request.Form.TryGetValue(name,out var form))return form.Count>0?form[0]:null;
  return null;
 }
}
