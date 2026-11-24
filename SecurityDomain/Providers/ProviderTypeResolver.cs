namespace SemBackDotNet10.SecurityDomain.Providers
{
    public static class ProviderTypeResolver
    {
        public static string Normalize(string? type)
        {
            return type?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        public static bool IsJdbc(string? type)
        {
            return Normalize(type) is "JDBC" or "DAO";
        }

        public static bool IsLdap(string? type)
        {
            return Normalize(type) is "LDAP" or "LDAPOPENLDAP";
        }

        public static bool IsOidc(string? type)
        {
            return Normalize(type) == "OIDC";
        }
    }
}