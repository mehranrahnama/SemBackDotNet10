using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

public class OAuth2UserInfo
{
    public OAuth2UserInfo(
        IDictionary<string, object?> attributes,
        OauthProvider oauthProvider,
        IDictionary<string, object?>? additionalAttributes = null)
    {
        Attributes = new Dictionary<string, object?>(attributes);
        AdditionalAttributes = additionalAttributes is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(additionalAttributes);
        OauthProvider = oauthProvider ?? throw new ArgumentNullException(nameof(oauthProvider));
    }

    public OAuth2UserInfo(
        IDictionary<string, object?> attributes,
        IDictionary<string, object?> additionalAttributes,
        OauthProvider oauthProvider)
        : this(attributes, oauthProvider, additionalAttributes)
    {
    }

    public IDictionary<string, object?> Attributes { get; set; }
    public IDictionary<string, object?> AdditionalAttributes { get; set; }
    public OauthProvider OauthProvider { get; }

    public string? UserName => GetValue(OauthProvider.UsernameAttr);
    public string? Name => GetValue(OauthProvider.NameAttr);
    public string? Family => GetValue(OauthProvider.FamilyAttr);
    public string? Email => GetValue(OauthProvider.MailAttr);

    public string? GetUserName() => UserName;
    public string? GetName() => Name;
    public string? GetFamily() => Family;
    public string? GetEmail() => Email;

    private string? GetValue(string? key) =>
        !string.IsNullOrWhiteSpace(key) && Attributes.TryGetValue(key, out var value)
            ? Convert.ToString(value)
            : null;
}
