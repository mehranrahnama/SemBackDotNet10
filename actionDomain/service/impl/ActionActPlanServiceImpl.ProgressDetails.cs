using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public ActionProgressDto? getActionActPlan(string? body)
    {
        try
        {
            using var json = JsonDocument.Parse(body!);
            long? ReadId(string name) => json.RootElement.TryGetProperty(name, out var item)
                && item.ValueKind != JsonValueKind.Null ? item.GetInt64() : null;
            var rows = _actionActPlanRepository.FindProgressDetails(ReadId("actionActPlanId"), ReadId("planId"));
            if (rows.Count == 0) return null;
            var first = rows[0];
            if (first.AddToSupremeLevel && first.ActionPlanCatalogId is long catalogId)
                first.DesLevel = string.Join("، ", _actionActPlanRepository.FindConnectedCommitteeNames(catalogId));
            if (rows.Count > 1)
                first.HadafName = string.Concat(rows.Select(row => (row.HadafName ?? "null") + "/"));
            AddGridActivities(new List<ActionProgressDto> { first });
            return first;
        }
        catch (JsonException error)
        {
            throw new BusinessException(error.Message);
        }
    }
}
