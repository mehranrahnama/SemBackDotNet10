
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class OauthProviderDto
{
    private static readonly string PRE_AUTHORIZE_URI = "/oauth2/authorize/";
    private static readonly string REDIRECT_URL_PARAM = "?redirect_uri=";

    public long? OrganId { get; set; }

    public string? Url { get; set; }

    public string? Name { get; set; }

    public string? RegistrationId { get; set; }

    public OauthProviderDto(OauthProvider oauthProvider)
    {
        OrganId = oauthProvider.Tenant != null
            ? oauthProvider.Tenant.Id
            : null;

        RegistrationId = oauthProvider.RegistrationId;
    }

    public OauthProviderDto(
        OauthProvider oauthProvider,
        string redirectUri,
        string serverAddress)
    {
        Url = oauthProvider.Type == OauthProvider.ProviderType.OIDC.ToString()
            ? serverAddress + PRE_AUTHORIZE_URI
                + oauthProvider.RegistrationId
                + REDIRECT_URL_PARAM
                + redirectUri
            : null;

        Name = oauthProvider.Tenant.TenantName;

        RegistrationId = oauthProvider.RegistrationId;

        OrganId = oauthProvider.Tenant != null
            ? oauthProvider.Tenant.Id
            : null;
    }

    public OauthProviderDto(
        string? url,
        string? name,
        string? registrationId,
        long? organId)
    {
        Url = url;
        Name = name;
        RegistrationId = registrationId;
        OrganId = organId;
    }

    public static string GetPreAuthorizeUri()
    {
        return PRE_AUTHORIZE_URI;
    }

    public static string GetRedirectUrlParam()
    {
        return REDIRECT_URL_PARAM;
    }
}
