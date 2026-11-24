using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل CorrectionActionsByEMS.java — @Table(name = "tblcorrectionactionsbyems", schema = "dbo").
/// lombok @Builder/@Getter/@Setter/@NoArgsConstructor/@AllArgsConstructor در C# نیازی ندارد؛
/// propertyهای خودکار همان کار getter/setter را انجام می‌دهند و object initializer
/// معادل builder است.
/// </summary>
[Table("tblcorrectionactionsbyems", Schema = "dbo")]
public class CorrectionActionsByEMS
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    /// <summary>@Basic @Column(nullable = false, name = "committeeid")</summary>
    [Column("committeeid")]
    public long CommitteeId { get; set; }

    /// <summary>@Basic @Column(nullable = false, name = "committeename")</summary>
    [Column("committeename")]
    public string CommitteeName { get; set; } = null!;

    [Column("status")]
    public string Status { get; set; } = null!;

    /// <summary>@Basic @Column(nullable = false, name = "identitycode")</summary>
    [Column("identitycode")]
    public long IdentityCode { get; set; }

    /// <summary>@Column(name = "emsid")</summary>
    [Column("emsid")]
    public long? EmsId { get; set; }
}
