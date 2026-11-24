
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    [Table("tbl_auth_provider", Schema = "dbo")]
    public class OauthProvider : AuditEntity<long>
    {
        public const string SEM = "SEM";

        // ----------------------------
        // Primary Key
        // ----------------------------
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        // ----------------------------
        // OAuth / OIDC Config
        // ----------------------------
        [Column("issueruri")]
        public string? IssuerUri { get; set; }

        [Column("tokenuri")]
        public string? TokenUri { get; set; }

        [Column("authorizationuri")]
        public string? AuthorizationUri { get; set; }

        [Column("jwkseturi")]
        public string? JwkSetUri { get; set; }

        [Column("userinfouri")]
        public string? UserInfoUri { get; set; }

        [Column("clientid")]
        public string? ClientId { get; set; }

        [Column("clientsecret")]
        public string? ClientSecret { get; set; }

        [Column("redirecturi")]
        public string? RedirectUri { get; set; }

        [Column("authorizationgranttype")]
        public string? AuthorizationGrantType { get; set; }

        [Column("clientauthenticationmethod")]
        public string? ClientAuthenticationMethod { get; set; }

        [Column("scope")]
        public string? Scope { get; set; }

        [Column("registration_id")]
        public string? RegistrationId { get; set; }

        // ----------------------------
        // User Attribute Mapping
        // ----------------------------
        [Column("usernameattr")]
        public string? UsernameAttr { get; set; }

        [Column("nameattr")]
        public string? NameAttr { get; set; }

        [Column("familyattr")]
        public string? FamilyAttr { get; set; }

        [Column("mailattr")]
        public string? MailAttr { get; set; }

        // ----------------------------
        // Tenant (OneToOne)
        // ----------------------------
        [ForeignKey("Tenant")]
        [Column("tenantid")]
        public long? TenantId { get; set; }

        public virtual Tenant? Tenant { get; set; }

        // ----------------------------
        // Accounts (OneToMany)
        // ----------------------------
        public virtual ICollection<UserAccount>? Accounts { get; set; }

        // ----------------------------
        // Provider Type
        // ----------------------------
        [Required]
        [Column("type")]
        public string Type { get; set; } = null!;

        // ----------------------------
        // SAHA
        // ----------------------------
        [Required]
        [Column("is_saha_based")]
        public bool IsSAHABased { get; set; }

        // ----------------------------
        // LDAP Config
        // ----------------------------
        [Required]
        [Column("ldap_url")]
        public string LdapUrl { get; set; } = null!;

        [Required]
        [Column("ldap_base_dn")]
        public string LdapBaseDn { get; set; } = null!;

        [Required]
        [Column("ldap_manager_dn")]
        public string LdapManagerDn { get; set; } = null!;

        [Required]
        [Column("ldap_manager_password")]
        public string LdapManagerPassword { get; set; } = null!;

        [Required]
        [Column("search_filter")]
        public string SearchFilter { get; set; } = null!;

        // ----------------------------
        // Enum Mapping
        // ----------------------------
        public enum ProviderType
        {
            DAO,
            LDAP,
            OIDC
        }
    }
}
