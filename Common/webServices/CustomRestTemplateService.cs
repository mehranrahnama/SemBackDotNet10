using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
namespace Common.webServices;

// Inject a host-configured HttpClient (timeouts, TLS, trusted destinations, credentials).
public sealed class CustomRestTemplateService(HttpClient client,ILogger<CustomRestTemplateService> logger)
{
    private async Task<JsonObject?> Exchange(HttpRequestMessage request,HttpStatusCode expected,string error,CancellationToken cancellationToken,bool unwrapArray=false)
    {
        try
        {
            using var response=await client.SendAsync(request,cancellationToken);
            response.EnsureSuccessStatusCode();
            if(response.StatusCode!=expected)return null;
            var text=await response.Content.ReadAsStringAsync(cancellationToken);
            if(unwrapArray)
            {
                // Preserve the source contract: remove the first and last character,
                // then require a JSON object (only a single-element object array works).
                if(text.Length<2)throw new JsonException("Expected a wrapped JSON object.");
                text=text[1..^1];
            }
            return JsonNode.Parse(text) as JsonObject ?? throw new JsonException("Expected a JSON object.");
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(ex,"HTTP exchange failed");
            throw new InvalidOperationException(error,ex);
        }
    }
    public async Task<JsonObject?> SignInRestApi(string url,string userName,string password,CancellationToken cancellationToken=default)
    {
        using var request=new HttpRequestMessage(HttpMethod.Post,url){Content=Json(new Dictionary<string,string>{{"username",userName},{"password",password}})};
        return await Exchange(request,HttpStatusCode.Created,"restTemplate-exchange throw exception",cancellationToken);
    }
    // Java passes this map as HttpEntity BODY, not headers. That unusual contract is preserved.
    public async Task<JsonNode?> SignInRestApi(string url,IReadOnlyDictionary<string,string> headers,CancellationToken cancellationToken=default)
    {
        using var request=new HttpRequestMessage(HttpMethod.Get,url){Content=Json(headers)};
        try
        {
            using var response=await client.SendAsync(request,cancellationToken);
            response.EnsureSuccessStatusCode();
            return response.StatusCode==HttpStatusCode.OK ? JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken)) : null;
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(ex,"HTTP sign-in failed");
            throw new InvalidOperationException("restTemplate-exchange throw exception",ex);
        }
    }
    public async Task<JsonObject?> PostExchange(string uri,object? body,IReadOnlyDictionary<string,IEnumerable<string>> headers,CancellationToken cancellationToken=default)
    {
        using var request=new HttpRequestMessage(HttpMethod.Post,uri){Content=Json(body)};
        AddHeaders(request,headers);
        return await Exchange(request,HttpStatusCode.Created,"restTemplate-postExchange throw exception",cancellationToken);
    }
    public async Task<JsonObject?> GetExchange(string uri,object body,IReadOnlyDictionary<string,IEnumerable<string>> headers,CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(body);
        var builder=new UriBuilder(uri);
        var query=builder.Query.TrimStart('?');
        builder.Query=(query.Length==0?"":query+"&")+"id="+Uri.EscapeDataString(body.ToString()!);
        using var request=new HttpRequestMessage(HttpMethod.Get,builder.Uri){Content=new StringContent(body.ToString()!,Encoding.UTF8)};
        AddHeaders(request,headers);
        return await Exchange(request,HttpStatusCode.OK,"restTemplate-GetExchange throw exception",cancellationToken,true);
    }
    private static StringContent Json(object? value)=>new(JsonSerializer.Serialize(value),Encoding.UTF8,"application/json");
    private static void AddHeaders(HttpRequestMessage request,IReadOnlyDictionary<string,IEnumerable<string>> headers)
    {
        foreach(var (key,values) in headers)
        {
            if(request.Headers.TryAddWithoutValidation(key,values))continue;
            request.Content!.Headers.Remove(key);
            if(!request.Content.Headers.TryAddWithoutValidation(key,values))throw new ArgumentException($"Unsupported HTTP header: {key}");
        }
    }
}
