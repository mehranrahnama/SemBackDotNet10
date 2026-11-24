namespace SemBackDotNet10.dashboardDomain.model.dto;

public class DashboardActionPlanDto
{
    public int Id { get; set; }
    public string? ActionName { get; set; }
    public int ActionPlanId { get; set; }
    public int PlanId { get; set; }
    public string? DesLevel { get; set; }
    public int UserId { get; set; }
    public int Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? AchievedPCNNormal { get; set; }
    public decimal? Weight { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public decimal? Pcn { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? PerfectExtreme { get; set; }
}
