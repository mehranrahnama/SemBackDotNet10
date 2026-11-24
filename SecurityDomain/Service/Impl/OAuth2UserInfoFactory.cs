using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.SemUserDetail;
using SemBackDotNet10.SecurityDomain.Service.Interf;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل OAuth2UserInfoFactory.java — کلاس استاتیک factory.
/// بلاک reflection کامنت‌شده در جاوا حذف شد؛ منطق فعال فقط ساخت OAuth2UserInfo است.
/// وابستگی‌های اعلام‌شده: AuthenticationProcessingException، OauthProvider (entity)،
/// OAuth2UserInfo، IOauthProviderService (هنوز تبدیل نشده‌اند).
/// </summary>
public static class OAuth2UserInfoFactory
{
    private const string OAuth2ClassKey = "com.irisaco.mss.sem.securityDomain.model.semUserDetail.oauth2userinfo";

    public static OAuth2UserInfo GetOAuth2UserInfo(
        string? registrationId,
        Dictionary<string, object?> attributes,
        Dictionary<string, object?> additionalAttributes,
        IOauthProviderService oauthProviderService)
    {
        if (!string.IsNullOrWhiteSpace(registrationId))
        {
            var oauthProvider = oauthProviderService.FindByRegistrationIdAsync(registrationId).GetAwaiter().GetResult();
            return new OAuth2UserInfo(attributes, additionalAttributes, oauthProvider);
        }
        throw new AuthenticationProcessingException("error.oauth2provider.providerNotFound", registrationId);
    }
}
