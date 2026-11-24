#nullable enable
using System;
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class SystemConfigurationDto
{
    public long? Id { get; set; }
    public long? TenantId { get; set; }
    public bool MultiTargetActionPlan { get; set; }
    public string? MeasureTrend { get; set; }

    public SystemConfigurationDto() { }

    public SystemConfigurationDto(long? id, long? tenantId, bool multiTargetActionPlan, string? measureTrend)
    {
        Id = id;
        TenantId = tenantId;
        MultiTargetActionPlan = multiTargetActionPlan;
        MeasureTrend = measureTrend;
    }
}
