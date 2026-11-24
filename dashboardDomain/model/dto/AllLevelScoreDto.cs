namespace SemBackDotNet10.dashboardDomain.model.dto;

public class AllLevelScoreDto
{
    // Request body
    public long? PlanId { get; set; }
    public long? Month { get; set; }
    public long? LevelId { get; set; }
    public long? FetchType { get; set; }

    // Response
    public string? DesLevel { get; set; }
    public string? DesOrganization { get; set; }
    public decimal? ScoreActualSelf { get; set; }
    public decimal? ScoreEval { get; set; }
    public int? ObjectCountGreen { get; set; }
    public int? ObjectCountYellow { get; set; }
    public int? ObjectCountRed { get; set; }
    public int? ObjectCountGray { get; set; }

    public override bool Equals(object? obj) =>
        obj is AllLevelScoreDto o && PlanId == o.PlanId && Month == o.Month && LevelId == o.LevelId
        && FetchType == o.FetchType && DesLevel == o.DesLevel && DesOrganization == o.DesOrganization
        && ScoreActualSelf == o.ScoreActualSelf && ScoreEval == o.ScoreEval
        && ObjectCountGreen == o.ObjectCountGreen && ObjectCountYellow == o.ObjectCountYellow
        && ObjectCountRed == o.ObjectCountRed && ObjectCountGray == o.ObjectCountGray;

    public override int GetHashCode() =>
        HashCode.Combine(PlanId, Month, LevelId, FetchType, DesLevel, DesOrganization, ScoreActualSelf, ScoreEval);
}
