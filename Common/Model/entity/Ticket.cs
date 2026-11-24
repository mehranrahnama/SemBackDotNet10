// C# migration of model/entity/Ticket.java

namespace Common.model.entity;

[Table("tblticket", Schema = "dbo")]
public class Ticket
{
    [Column("TicketId")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}
