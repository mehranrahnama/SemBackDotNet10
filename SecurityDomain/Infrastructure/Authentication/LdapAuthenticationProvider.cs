using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Providers;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class LdapAuthenticationProvider(
    string type,
    IUserAccountRepository accounts,
    IRoleRepository roles,
    ILogger<LdapAuthenticationProvider> logger) : IAuthenticationProvider
{
    public string Type => type;

    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthProviderEntity provider, AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        var failed = AuthenticationResult.Failed(request.TenantId, request.Username, "INVALID_CREDENTIALS");
        if (provider.TenantId != request.TenantId || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrWhiteSpace(provider.RegistrationId))
            return failed;

        var attributes = new[] { "uid" }.Concat((provider.SearchFilter ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (attributes.Any(attribute => !Regex.IsMatch(attribute, @"^[a-zA-Z][a-zA-Z0-9-]*$")))
        {
            logger.LogWarning("Invalid LDAP search attribute for provider {ProviderId}", provider.Id);
            return failed;
        }

        var value = EscapeFilter(request.Username);
        var filter = "(|" + string.Concat(attributes.Select(attribute => $"({attribute}={value})")) + ")";
        foreach (var address in (provider.LdapUrl ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "ldap" && uri.Scheme != "ldaps"))
            {
                logger.LogWarning("Invalid LDAP URL for provider {ProviderId}", provider.Id);
                continue;
            }

            try
            {
                using var connection = new LdapConnection(new LdapDirectoryIdentifier(uri.Host, uri.IsDefaultPort ? (uri.Scheme == "ldaps" ? 636 : 389) : uri.Port))
                {
                    AuthType = AuthType.Basic,
                    Timeout = TimeSpan.FromSeconds(5)
                };
                connection.SessionOptions.ProtocolVersion = 3;
                connection.SessionOptions.SecureSocketLayer = uri.Scheme == "ldaps";
                if (!string.IsNullOrWhiteSpace(provider.LdapManagerDn) && provider.LdapManagerPassword != null)
                    connection.Bind(new NetworkCredential(provider.LdapManagerDn, provider.LdapManagerPassword));
                else
                {
                    connection.AuthType = AuthType.Anonymous;
                    connection.Bind();
                }

                var search = new SearchRequest(provider.LdapBaseDn ?? "", filter, SearchScope.Subtree, "1.1") { SizeLimit = 2 };
                var result = (SearchResponse)connection.SendRequest(search);
                if (result.Entries.Count != 1)
                    continue;

                connection.AuthType = AuthType.Basic;
                connection.Bind(new NetworkCredential(result.Entries[0].DistinguishedName, request.Password));

                var accountName = request.Username + "_" + provider.RegistrationId;
                var account = await accounts.FindByProviderAndUsernameAsync(request.TenantId, provider.Id, accountName, cancellationToken);
                if (account?.ProviderId != provider.Id || account.User is null ||
                    account.User.TenantId != request.TenantId || account.User.LoginDisabled == true || account.User.Retired == true)
                    return failed;

                var userRoles = await roles.GetForUserAsync(request.TenantId, account.User.UserId, cancellationToken);
                return AuthenticationResult.Success(request.TenantId, account.AccountName,
                    string.Join(' ', new[] { account.User.NameF, account.User.FamilyF }.Where(part => !string.IsNullOrWhiteSpace(part))),
                    ProviderTypeResolver.Normalize(provider.Type), account.User.UserId, account.Id,
                    userRoles.Select(role => role.Name).ToArray(), userRoles.Select(role => role.Id).ToArray());
            }
            catch (Exception exception) when (exception is LdapException or DirectoryOperationException)
            {
                logger.LogWarning("LDAP authentication or connection failed for provider {ProviderId}: {ExceptionType}", provider.Id, exception.GetType().Name);
            }
        }

        return failed;
    }

    private static string EscapeFilter(string value)
    {
        var escaped = new StringBuilder();
        foreach (var character in value)
            escaped.Append(character switch { '\\' => "\\5c", '*' => "\\2a", '(' => "\\28", ')' => "\\29", '\0' => "\\00", _ => character.ToString() });
        return escaped.ToString();
    }
}
