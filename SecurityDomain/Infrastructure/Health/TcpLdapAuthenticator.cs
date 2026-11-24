using System.Net.Sockets;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Health;

public sealed class TcpLdapAuthenticator : ILdapAuthenticator
{
    public async Task CheckConnectionAsync(AuthProviderEntity provider, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(provider.LdapUrl, UriKind.Absolute, out var uri) ||
            string.IsNullOrWhiteSpace(uri.Host))
            throw new InvalidOperationException("LDAP provider URL is invalid.");

        var port = uri.Port > 0 ? uri.Port : uri.Scheme.Equals("ldaps", StringComparison.OrdinalIgnoreCase) ? 636 : 389;
        using var client = new TcpClient();
        await client.ConnectAsync(uri.Host, port, cancellationToken);
    }
}
