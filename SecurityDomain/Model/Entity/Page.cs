using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

/// <summary>
/// معادل Page.java — جدول dbo.tblpage.
/// وابستگی‌های اعلام‌شده: AuditEntity&lt;TId&gt; (base model — هنوز تبدیل نشده).
/// طول‌های ستون‌ها (length = ...) اینجا به‌صورت [MaxLength] حفظ شدند؛ maxlength = 2147483647
/// یعنی MAX و با نوع string عادی پوشش داده می‌شود.
/// </summary>
[Table("tblpage", Schema = "dbo")]
public class Page : AuditEntity<long>
{
    [Key]
    [Column("pageid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    [Column("isdefault")]
    public bool? IsDefault { get; set; }

    [Column("parentid")]
    public int ParentId { get; set; }

    [Column("desf")]
    [MaxLength(60)]
    public string DesF { get; set; } = null!;

    [Column("dese")]
    [MaxLength(50)]
    public string? DesE { get; set; }

    [Column("last")]
    public int? Last { get; set; }

    [Column("uri")]
    [MaxLength(200)]
    public string Uri { get; set; } = null!;

    [Column("clfdes")]
    public string? ClfDes { get; set; }

    [Column("seq")]
    public int Seq { get; set; }

    [Column("path")]
    [MaxLength(200)]
    public string Path { get; set; } = null!;

    [Column("pagename")]
    [MaxLength(100)]
    public string PageName { get; set; } = null!;

    [Column("visible")]
    public bool? Visible { get; set; }

    [Column("code")]
    [MaxLength(20)]
    public string? Code { get; set; }

    [Column("imageurl")]
    [MaxLength(50)]
    public string? ImageUrl { get; set; }

    [Column("applicationname")]
    [MaxLength(256)]
    public string ApplicationName { get; set; } = null!;

    /// <summary>@Transient — نگاشت به جدول ندارد و فقط برای ساخت درخت منو استفاده می‌شود.</summary>
    [NotMapped]
    public List<Page>? Children { get; set; }
}
