using System.Text.Json.Serialization;

namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class ControlPanelDto
{
    public long? Id { get; set; }
    public string? PlaningLevelDes { get; set; }
    public bool? Alllevelsallowedspec { get; set; }
    public long? Levelidallowedspec { get; set; }
    public bool? Alltashilgarsallowedspec { get; set; }
    public long? Tashilgaridallowedspec { get; set; }
    public string? UserName { get; set; }
    public int? Entitytype { get; set; }
    public string? Regdatestart { get; set; }
    public string? Regdateend { get; set; }
    public string? Valdatestart { get; set; }
    public string? Valdateend { get; set; }
    public string? Monthvalspec { get; set; }
    public string? Yearvalspec { get; set; }
    public string? AllowedPlaningLevelIds { get; set; }

    [JsonPropertyName("val_month")]
    public string? Val_month { get; set; }

    public ControlPanelDto()
    {
    }

    public ControlPanelDto(long? id, string? planingLevelDes, string? userName, string? regdatestart,
        string? regdateend, string? valdatestart, string? valdateend, string? yearvalspec, string? monthvalspec,
        int? entitytype, bool? alllevelsallowedspec, long? levelidallowedspec, bool? alltashilgarsallowedspec,
        long? tashilgaridallowedspec, string? val_month, string? allowedPlaningLevelIds)
    {
        Id = id;
        PlaningLevelDes = planingLevelDes;
        Alllevelsallowedspec = alllevelsallowedspec;
        Levelidallowedspec = levelidallowedspec;
        Alltashilgarsallowedspec = alltashilgarsallowedspec;
        Tashilgaridallowedspec = tashilgaridallowedspec;
        UserName = userName;
        Entitytype = entitytype;
        Regdatestart = regdatestart;
        Regdateend = regdateend;
        Valdatestart = valdatestart;
        Valdateend = valdateend;
        Monthvalspec = monthvalspec;
        Yearvalspec = yearvalspec;
        Val_month = val_month;
        AllowedPlaningLevelIds = allowedPlaningLevelIds;
    }
}
