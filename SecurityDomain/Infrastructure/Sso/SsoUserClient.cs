using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Sso;

public sealed class SsoUserClient(HttpClient httpClient) : ISsoUserClient
{
    public async Task<SsoUserDto> GetUserAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("An SSO access token is required.", nameof(accessToken));

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1.0/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new SsoUserNotFoundException();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SsoUserDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("SSO returned an empty response.");
    }
}

public sealed class SsoUserNotFoundException() : Exception("The SSO user was not found.");
