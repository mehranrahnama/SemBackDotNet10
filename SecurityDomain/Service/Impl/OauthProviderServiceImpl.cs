using Common.exception;
using Common.logging;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
using SemBackDotNet10.SecurityDomain.Service.Interf;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل OauthProviderServiceImpl.java.
/// نشانی محلی از نام میزبان سیستم به دست می‌آید، مانند InetAddress.getLocalHost در Java.
/// @Valueها → کلیدهای IConfiguration (نقطه → دو نقطه).
/// </summary>
public class OauthProviderServiceImpl : IOauthProviderService
{
    private static readonly Lazy<string> LocalSystemIp = new(ResolveLocalSystemIp);

    private static string ResolveLocalSystemIp()
    {
        try
        {
            return System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).First().ToString();
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            throw new BusinessException(ex.Message);
        }
    }

    private readonly IConfiguration _configuration;
    private readonly IOauthProviderRepository _oauthProviderRepository;
    private readonly SemLogger _semLogger;

    public OauthProviderServiceImpl(
        IConfiguration configuration,
        IOauthProviderRepository oauthProviderRepository,
        SemLogger semLogger)
    {
        _configuration = configuration;
        _oauthProviderRepository = oauthProviderRepository;
        _semLogger = semLogger;
    }

    /// <summary>معادل getRepository().</summary>
    public IOauthProviderRepository GetRepository() => _oauthProviderRepository;

    public async Task<OauthProvider> FindByIdOrElseThrowAsync(long id, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        if (id == 0)
        {
            throw new BusinessException("error.oauthProvider.idIsNotValid");
        }
        var result = await _oauthProviderRepository.FindByIdAsync(id, ct)
                     ?? throw new BusinessException("error.oauthProvider.idIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }

    public async Task DeleteOauthProviderAsync(long id, CancellationToken ct = default)
    {
        var oauthProvider = await FindByIdOrElseThrowAsync(id, ct);
        await _oauthProviderRepository.DeleteAsync(oauthProvider, ct);
    }

    /// <summary>معادل findAllProviderInfo با @Transactional(readOnly = true).</summary>
    public async Task<List<OauthProviderDto>> FindAllProviderInfoAsync(CancellationToken ct = default)
    {
        var oauthProviders = await GetRepository().FindAllAsync(ct);

        var redirectUri = "http://" + LocalSystemIp.Value
                          + _configuration["sem:oauth2:authorizedRedirectUris"];
        var server = _configuration["sem:server:address"];
        var port = _configuration["server:port"];
        var contextPath = _configuration["server:servlet:context-path"];
        var address = port == "80" ? server + contextPath : server + ":" + port + contextPath;

        var result = oauthProviders
            .Where(oauthProvider => oauthProvider.TenantId != null
                                    && !oauthProvider.Type.Equals(OauthProvider.ProviderType.DAO.ToString(), StringComparison.Ordinal))
            .Select(oauthProvider => new OauthProviderDto(oauthProvider, redirectUri, address!))
            .ToList();
        return result;
    }

    public async Task<OauthProvider> FindByRegistrationIdAsync(string registrationId, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        if (registrationId is null)
        {
            throw new BusinessException("error.oauthProvider.registrationIdIsNotValid");
        }
        var result = await _oauthProviderRepository.FindByRegistrationIdAsync(registrationId, ct)
                     ?? throw new BusinessException("error.oauthProvider.registrationIdIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }

    public async Task<OauthProvider?> FindByTenantIdAndTypeAsync(long tenantId, string type, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var result = await _oauthProviderRepository.FindByTenantIdAndTypeAsync(tenantId, type, ct);
        _semLogger.Info("Exit method");
        return result;
    }

    public async Task<OauthProvider> FindByTypeAsync(string type, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var result = await _oauthProviderRepository.FindByTypeAsync(type, ct)
                     ?? throw new BusinessException("error.oauthProvider.registrationIdIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }

    public async Task<OauthProvider?> FindLdapProviderAsync(CancellationToken ct = default)
    {
        return await _oauthProviderRepository.FindByTypeAsync("LDAP", ct);
    }
}
