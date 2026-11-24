namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    public sealed class AuthProviderEntity
    {
        public long Id { get; set; }

        public long TenantId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string? LdapUrl { get; set; }

        public string? LdapBaseDn { get; set; }

        public string? LdapManagerDn { get; set; }

        public string? LdapManagerPassword { get; set; }

        public string? SearchFilter { get; set; }

        public bool? IsSahaBased { get; set; }

        public string? IssuerUri { get; set; }

        public string? TokenUri { get; set; }

        public string? AuthorizationUri { get; set; }

        public string? JwkSetUri { get; set; }

        public string? UserInfoUri { get; set; }

        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }

        public string? RedirectUri { get; set; }

        public string? AuthorizationGrantType { get; set; }

        public string? ClientAuthenticationMethod { get; set; }

        public string? Scope { get; set; }

        public string? RegistrationId { get; set; }
    }
}
