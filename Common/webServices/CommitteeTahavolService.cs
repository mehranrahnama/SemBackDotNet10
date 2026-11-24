using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Common.exception;
namespace Common.webServices;
public sealed record CommitteeTahavolOptions(string TokenServiceUrl,string UserName,string Password,string CommitteeTahavolUrl);
// HttpClient destination restrictions, TLS and timeouts must be configured by host.
public sealed class CommitteeTahavolService(HttpClient client, CustomRestTemplateService authentication, CommitteeTahavolOptions options)
{
    public async Task<string?> GetToken(CancellationToken cancellationToken=default)
    {
        try
        {
            var response=await authentication.SignInRestApi(options.TokenServiceUrl,options.UserName,options.Password,cancellationToken);
            return response is null ? null : response["accessToken"]?.ToString() ?? throw new InvalidOperationException("accessToken not found");
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested){throw;}
        catch(Exception){throw new BusinessException("sth became wrong");}
    }
    public async Task<JsonArray> GetCommitteeTahavol(CancellationToken cancellationToken=default)
    {
        try
        {
            var token=await GetToken(cancellationToken);
            if(string.IsNullOrEmpty(token))throw new InvalidOperationException("دریافت توکن با خطا مواجه شده است");
            using var request=new HttpRequestMessage(HttpMethod.Get,options.CommitteeTahavolUrl);
            request.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
            request.Content=new ByteArrayContent([]);
            request.Content.Headers.ContentType=new MediaTypeHeaderValue("application/json");
            using var response=await client.SendAsync(request,cancellationToken);
            response.EnsureSuccessStatusCode();
            return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken)) as JsonArray ?? throw new InvalidOperationException("Expected a JSON array");
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested){throw;}
        catch(Exception ex){throw new BusinessException(ex.Message);}
    }
}
