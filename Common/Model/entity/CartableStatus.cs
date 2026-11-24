// C# migration of model/entity/CartableStatus.java

namespace Common.model.entity;

[Table("tblcartablestatus", Schema = "dbo")]
public class CartableStatus
{
    [Column("Cartablestatusid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("Entityname")] [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [Column("Outcome")] [JsonPropertyName("outCome")]
    public string? OutCome { get; set; }
    [Column("Oldstatus")] [JsonPropertyName("oldStatus")]
    public long? OldStatus { get; set; }
    [Column("Newstatus")] [JsonPropertyName("newStatus")]
    public long? NewStatus { get; set; }
    [Column("operationtype")] [JsonPropertyName("operationType")]
    public string? OperationType { get; set; }
    [Column("fromroleid")]
    public long? FromRoleId { get; set; }
    [ForeignKey(nameof(FromRoleId)), JsonPropertyName("fromRole")]
    public SemBackDotNet10.SecurityDomain.Model.Entity.Role? FromRole { get; set; }
    [Column("toroleid")]
    public long? ToRoleId { get; set; }
    [ForeignKey(nameof(ToRoleId)), JsonPropertyName("toRole")]
    public SemBackDotNet10.SecurityDomain.Model.Entity.Role? ToRole { get; set; }
    [Column("oldstatusmeaning")] [JsonPropertyName("oldStatusMeaning")]
    public string? OldStatusMeaning { get; set; }
    [Column("newstatusmeaning")] [JsonPropertyName("NewStatusMeaning")]
    public string? NewStatusMeaning { get; set; }
}
