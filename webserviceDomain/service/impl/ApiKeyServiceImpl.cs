using System.Security.Cryptography;
using System.Text;
using SemBackDotNet10.webserviceDomain.repository;
using SemBackDotNet10.webserviceDomain.service.interf;

namespace SemBackDotNet10.webserviceDomain.service.impl;

public class ApiKeyServiceImpl : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyServiceImpl(IApiKeyRepository apiKeyRepository) => _apiKeyRepository = apiKeyRepository;

    public async Task<bool> ValidateApiCredentialsAsync(string? apiKey, string? apiHash, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(apiKey) || apiHash == null)
            return false;

        var apiKeys = await _apiKeyRepository.FindByApiKeyAsync(apiKey, ct);
        return apiKeys != null &&
               CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(apiKeys.ApiHash), Encoding.UTF8.GetBytes(apiHash));
    }
}
