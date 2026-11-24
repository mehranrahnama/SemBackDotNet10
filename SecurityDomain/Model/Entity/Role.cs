using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity
{
    [Table("tblroles", Schema = "dbo")]
    public class Role : AuditEntity<long>
    {
        // ----------------------------
        // Primary Key
        // ----------------------------
        [Key]
        [Column("roleid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }

        // ----------------------------
        // Role Name
        // ----------------------------
        [Column("rolename", TypeName = "nvarchar(250)")]
        public string? RoleName { get; set; }

        // ----------------------------
        // Role Description
        // ----------------------------
        [Column("roledes", TypeName = "nvarchar(250)")]
        public string? RoleDes { get; set; }

        // ----------------------------
        // One-to-Many → UserRoles
        // ----------------------------
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();


        // ----------------------------
        // Application Roles Enum
        // ----------------------------
        public enum ApplicationRole
        {
            SEM_ADMIN,
            SEM_HELP_DEDSK,
            ADMINSTRATORS
        }
    }
}
