using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    [Table("tbluserroles", Schema = "dbo")]
    public class UserRole : AuditEntity<long>
    {
        // ----------------------------
        // Primary Key
        // ----------------------------
        [Key]
        [Column("userroleid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        // ----------------------------
        // Role Relation (Many-to-One)
        // ----------------------------
        [ForeignKey(nameof(RoleId))]
        [Column("roleid")]
        [Required]
        public long RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;

        // ----------------------------
        // User Relation (Many-to-One)
        // ----------------------------
        [ForeignKey(nameof(UserId))]
        [Column("userid")]
        [Required]
        public long UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
