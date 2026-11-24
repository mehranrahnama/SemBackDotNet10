using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblDelegation", Schema = "dbo")]
public class Delegation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("delegationId", TypeName = "int")]
    public long Id { get; set; }

    [Required]
    [Column("fromUser", TypeName = "int")]
    public long? FromUserId { get; set; }

    public User? FromUser { get; set; }

    [Required]
    [Column("toUser", TypeName = "int")]
    public long? ToUserId { get; set; }

    public User? ToUser { get; set; }

    [Required]
    [Column("delegationStartDate", TypeName = "datetime")]
    public DateTime? DelegationStartDate { get; set; }

    [Required]
    [Column("delegationEndDate", TypeName = "datetime")]
    public DateTime? DelegationEndDate { get; set; }

    [Column("delegationStatus", TypeName = "int")]
    public bool DelegationStatus { get; set; }
}
