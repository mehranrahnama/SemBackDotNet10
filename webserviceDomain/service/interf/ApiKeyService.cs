namespace SemBackDotNet10.webserviceDomain.service.interf;

public interface IApiKeyService
{
    Task<bool> ValidateApiCredentialsAsync(string? apiKey, string? apiHash, CancellationToken ct = default);
}
