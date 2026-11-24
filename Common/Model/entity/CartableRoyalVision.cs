// C# migration of model/entity/CartableRoyalVision.java

namespace Common.model.entity;

[Table("tblcartableroyalvision", Schema = "dbo")]
public class CartableRoyalVision
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("issmssent")]
    public bool Issmssent { get; set; }
    [JsonPropertyName("tashilgar")]
    public string? Tashilgar { get; set; }
    [JsonPropertyName("modir")]
    public string? Modir { get; set; }
    [JsonPropertyName("statusdes")]
    public string? Statusdes { get; set; }
    [JsonPropertyName("taskrow")]
    public long? Taskrow { get; set; }
    [JsonPropertyName("oldmeasureconfirmstatus")]
    public long? Oldmeasureconfirmstatus { get; set; }
    [JsonPropertyName("newmeasureconfirmstatus")]
    public long? Newmeasureconfirmstatus { get; set; }
    [JsonPropertyName("assignee")]
    public long? Assignee { get; set; }
    [JsonPropertyName("listid")]
    public long? Listid { get; set; }
    [JsonPropertyName("assigneddate")]
    public DateTime? Assigneddate { get; set; }
    [JsonPropertyName("persianassigneddate")]
    public string? Persianassigneddate { get; set; }
    [JsonPropertyName("assigneegroups")]
    public string? Assigneegroups { get; set; }
    [JsonPropertyName("assignees")]
    public string? Assignees { get; set; }
    [JsonPropertyName("assigneesdisplayname")]
    public string? Assigneesdisplayname { get; set; }
    [JsonPropertyName("reviewers")]
    public string? Reviewers { get; set; }
    [JsonPropertyName("reviewersdisplayname")]
    public string? Reviewersdisplayname { get; set; }
    [JsonPropertyName("creator")]
    public long? Creator { get; set; }
    [JsonPropertyName("fromuserdisplayname")]
    public string? Fromuserdisplayname { get; set; }
    [JsonPropertyName("createddate")]
    public DateTime? Createddate { get; set; }
    [JsonPropertyName("persiancreateddate")]
    public string? Persiancreateddate { get; set; }
    [JsonPropertyName("priority")]
    public long? Priority { get; set; }
    [JsonPropertyName("instanceid")]
    public string? Instanceid { get; set; }
    [JsonPropertyName("compositeinstanceid")]
    public string? Compositeinstanceid { get; set; }
    [JsonPropertyName("processid")]
    public string? Processid { get; set; }
    [JsonPropertyName("processname")]
    public string? Processname { get; set; }
    [JsonPropertyName("stage")]
    public string? Stage { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("tasknumber")]
    public string? Tasknumber { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    [JsonPropertyName("tasknamespace")]
    public string? Tasknamespace { get; set; }
    [JsonPropertyName("compositeversion")]
    public string? Compositeversion { get; set; }
    [JsonPropertyName("taskurl")]
    public string? Taskurl { get; set; }
}
