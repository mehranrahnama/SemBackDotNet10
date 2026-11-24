using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

/// <summary>
/// معادل PageAccess.java — جدول dbo.tblpageaccess.
/// وابستگی‌های اعلام‌شده: AuditEntity&lt;TId&gt;، Role (هر دو هنوز تبدیل نشده‌اند).
/// </summary>
[Table("tblpageaccess", Schema = "dbo")]
public class PageAccess : AuditEntity<long>
{
    [Key]
    [Column("pageaccessid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "pageid", referencedColumnName = "pageid")</summary>
    [ForeignKey(nameof(Page))]
    [Column("pageid")]
    public Page Page { get; set; } = null!;

    /// <summary>@ManyToOne @JoinColumn(name = "roleid", referencedColumnName = "roleid")</summary>
    [ForeignKey(nameof(Role))]
    [Column("roleid")]
    public Role Role { get; set; } = null!;
}
