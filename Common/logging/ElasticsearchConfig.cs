using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
namespace Common.logging;
public sealed record ElasticsearchConfig(Uri Endpoint,string Username,string Password,bool AllowInsecureHttp=false)
{
    public void ConfigureClient(HttpClient client)
    {
        if(!Endpoint.IsAbsoluteUri || (Endpoint.Scheme!="https" && !(AllowInsecureHttp && Endpoint.Scheme=="http")) ||
            !string.IsNullOrEmpty(Endpoint.UserInfo) || !string.IsNullOrEmpty(Endpoint.Query) || !string.IsNullOrEmpty(Endpoint.Fragment))
            throw new ArgumentException("Use an absolute HTTPS endpoint (HTTP requires explicit opt-in).");
        client.BaseAddress=new Uri(Endpoint.AbsoluteUri.TrimEnd('/')+"/");
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(Username+":"+Password)));
        // Host must use a handler with AllowAutoRedirect=false to prevent credential forwarding.
    }
    public static HttpClient CreateClient(ElasticsearchConfig settings)
    {
        var client=new HttpClient(new HttpClientHandler{AllowAutoRedirect=false});
        try {settings.ConfigureClient(client);return client;} catch {client.Dispose();throw;}
    }
}
public interface IElasticsearchSearchTransport
{
    Task<JsonObject> SearchAsync(string index,JsonObject request,CancellationToken cancellationToken);
}
public sealed class HttpElasticsearchSearchTransport(HttpClient client):IElasticsearchSearchTransport
{
    public async Task<JsonObject> SearchAsync(string index,JsonObject request,CancellationToken cancellationToken)
    {
        if(index!="logstash-*")throw new ArgumentException("Unsupported index.",nameof(index));
        using var content=new StringContent(request.ToJsonString(),Encoding.UTF8,"application/json");
        using var response=await client.PostAsync("logstash-*/_search",content,cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken)) as JsonObject
            ?? throw new InvalidDataException("Elasticsearch returned a non-object response.");
    }
}
