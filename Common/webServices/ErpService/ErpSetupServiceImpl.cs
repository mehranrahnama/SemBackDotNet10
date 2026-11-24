using System.Net.Http.Json;
using System.Text.Json;
using Common.exception;
using Common.webServices.ErpService.Interf;
namespace Common.webServices.ErpService;
// Inject a dedicated HttpClient: host owns TLS, timeout, redirects and outbound policy.
// Credentials must be supplied from host secrets, never logged or checked into source.
public sealed class ErpSetupServiceImpl(HttpClient client,string userName,string password):ErpSetupService
{
    public async Task<string> GetToken(CancellationToken cancellationToken=default)
    {
        try {
            using var response=await client.PostAsJsonAsync("https://frame.irisaco.com/api/auth/token/rest/v1.0",
                new Dictionary<string,string>{{"username",userName},{"password",password}},cancellationToken);
            response.EnsureSuccessStatusCode();
            var body=await response.Content.ReadAsStringAsync(cancellationToken);
            if(string.IsNullOrWhiteSpace(body)) return "null";
            using var document=JsonDocument.Parse(body);
            if(document.RootElement.ValueKind==JsonValueKind.Null) return "null";
            var token=document.RootElement.GetProperty("token");
            return token.ValueKind==JsonValueKind.String?token.GetString()!:token.ToString();
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested) { throw; }
        catch(Exception error) { throw new CustomException("ERP token request failed.",error); }
    }
    public async Task<ErpSmsResponse> CallErpSmsService(string message,IReadOnlyList<string> numbers,CancellationToken cancellationToken=default)
    {
        var token=await GetToken(cancellationToken);
        using var request=new HttpRequestMessage(HttpMethod.Post,ErpServicesHelper.CreateSmsServiceUrl());
        request.Headers.Authorization=ErpServicesHelper.SetTokenHeader(token);
        request.Content=JsonContent.Create(ErpServicesHelper.CreateSmsBody(message,numbers));
        using var response=await client.SendAsync(request,cancellationToken);
        // Java returns the status and string body even for HTTP error responses.
        return new((int)response.StatusCode,await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
