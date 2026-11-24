namespace SemBackDotNet10.webserviceDomain.domain.dto;

public class ActionPlanWebServiceDto
{
    public ActionPlanWebServiceDto()
    {
    }

    public ActionPlanWebServiceDto(
        long? actionId, string? actionPlanTitle, List<string>? objectives, string? startDate, string? dueDate,
        long? superVisorPersonId, string? superVisorPersonFullName, long? managerPersonId,
        string? managerPersonFullName, string? chartName, string? chartCode, float weight)
    {
        ActionId = actionId;
        ActionPlanTitle = actionPlanTitle;
        Objectives = objectives;
        StartDate = startDate;
        DueDate = dueDate;
        SuperVisorPersonId = superVisorPersonId;
        SuperVisorPersonFullName = superVisorPersonFullName;
        ManagerPersonId = managerPersonId;
        ManagerPersonFullName = managerPersonFullName;
        ChartName = chartName;
        ChartCode = chartCode;
        Weight = weight;
    }

    public long? ActionId { get; set; }
    public string? ActionPlanTitle { get; set; }
    public List<string>? Objectives { get; set; }
    public string? StartDate { get; set; }
    public string? DueDate { get; set; }
    public long? SuperVisorPersonId { get; set; }
    public string? SuperVisorPersonFullName { get; set; }
    public long? ManagerPersonId { get; set; }
    public string? ManagerPersonFullName { get; set; }
    public string? ChartName { get; set; }
    public string? ChartCode { get; set; }
    public float Weight { get; set; }
}
