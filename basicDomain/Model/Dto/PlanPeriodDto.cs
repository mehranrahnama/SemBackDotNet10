#nullable enable

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class PlanPeriodDto
{
    public int? Id { get; set; }
    public long? PlanId { get; set; }
    public short StatusType { get; set; }

    public PlanPeriodDto() { }

    public PlanPeriodDto(int id, long? planId, short statusType)
    {
        Id = id;
        PlanId = planId;
        StatusType = statusType;
    }
}
