using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    [Table("tbluseraccount", Schema = "dbo")]
    public class UserAccount : AuditEntity<long>
    {
        // ----------------------------
        // Primary Key
        // ----------------------------
        [Key]
        [Column("useraccountid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        // ----------------------------
        // Account Name
        // ----------------------------
        [Required]
        [Column("accountname", TypeName = "nvarchar(50)")]
        public string AccountName { get; set; } = null!;

        // ----------------------------
        // Account Type
        // ----------------------------
        [Column("accounttype")]
        public long? AccountType { get; set; }

        // ----------------------------
        // Password
        // ----------------------------
        [Required]
        [Column("password", TypeName = "nvarchar(50)")]
        public string Password { get; set; } = null!;

        // ----------------------------
        // User Relation
        // ----------------------------
        [ForeignKey("User")]
        [Column("userid")]
        public long? UserId { get; set; }

        public virtual User? User { get; set; }

        // ----------------------------
        // OAuth Provider
        // ----------------------------
        [ForeignKey("Provider")]
        [Column("providerid")]
        public long? ProviderId { get; set; }

        public virtual AuthProviderEntity? Provider { get; set; }

        // ----------------------------
        // OIDC User Info (JSON)
        // ----------------------------
        [Column("oidc_user_info", TypeName = "nvarchar(max)")]
        public string? OidcUserInfo { get; set; }
    }
}
