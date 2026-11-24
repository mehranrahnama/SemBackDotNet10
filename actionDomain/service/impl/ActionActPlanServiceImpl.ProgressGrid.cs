using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Common.utils;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public SearchDto loadGrid(Dictionary<string?, string?> headers, string? body, int page, int size,
        string? orderBy, SortDirection direction)
    {
        if (!_tenant.HasTenant)
            throw new UnAuthorizedException();
        using var json = JsonDocument.Parse(body!);
        var root = json.RootElement;
        long? Number(string key) => root.TryGetProperty(key, out var x) && x.ValueKind != JsonValueKind.Null ? x.GetInt64() : null;
        var filter = root.GetProperty("filter").GetString();
        var levelId = Number("levelId");
        var planId = Number("planId");
        var status = Number("status");
        var month = Number("month");
        var modirId = Number("modirId");
        var modirView = root.TryGetProperty("modirView", out var view) && view.ValueKind == JsonValueKind.True;
        var userId = SecurityUtils.GetCurrentUserId() ?? throw new BusinessException("کاربر جاری یافت نشد");
        var tenantId = _tenant.TenantId;
        if (!levelId.HasValue || !_actionActPlanRepository.GridLevelExists(levelId.Value, tenantId))
            throw new UnAuthorizedException();
        CheckPlanningLevelAccess(levelId);
        var admin = _userService.UserIsAdmin(userId);
        var levels = new List<long> { levelId.Value };
        if (modirView && (_userService.UserIsModir(userId) || admin))
            levels.AddRange(_actionActPlanRepository.FindDirectSublevels(levelId.Value, tenantId));
        var sanaye = _userRoleService.FindCurrentUserRolesName().Contains("SanayeUser");
        var rows = _actionActPlanRepository.FindProgressGrid(filter, levels, planId, status, month, modirId, userId, admin, sanaye, tenantId);
        var distinct = rows.GroupBy(x => x.ActionActPlanId).Select(x => x.First()).ToList();
        var request = PageUtils.GetInstance(page, size, mapOrderByToQueryOrderBy(orderBy), direction);
        var result = distinct.Skip(request.Offset).Take(request.Size).ToList();
        foreach (var row in result)
        {
            var plan = FindCorrectionPlan(row.ActionActPlanId);
            row.HadafName = string.Join("-", _actionActPlanRepository.FindGridStrategyTitles(
                plan.ActionPlanCatalog!.Id, plan.PlanPeriod!.PlanId, tenantId));
        }
        AddGridActivities(result);
        SetGridUploadedFiles(result);
        if (RequiredTenantName() == "HOSCO") ApplyGridEmsCorrections(headers, result);
        return new SearchDto.Builder().content(result).totalElements(distinct.Count)
            .totalPages((distinct.Count + request.Size - 1) / request.Size)
            .currentPage(page).pageSize(size).build();
    }

    private void ApplyGridEmsCorrections(Dictionary<string?, string?> headers,
        List<SemBackDotNet10.actionDomain.model.dto.ActionProgressDto> rows)
    {
        var corrections = (_correctionActionsByEMSService.retrieveAllCorrectionActionsByEMS(headers)
            ?? new List<SemBackDotNet10.actionDomain.model.dto.CorrectionActionByEMSDto>())
            .Where(x => x.IdentityCode.HasValue).GroupBy(x => x.IdentityCode!.Value)
            .ToDictionary(x => x.Key, x => x.First().Title);
        var changed = new List<SemBackDotNet10.actionDomain.model.entity.ActionActPlan>();
        foreach (var row in rows)
        {
            row.HasCorrectionEMS = row.ActionActPlanId.HasValue && corrections.ContainsKey(row.ActionActPlanId.Value);
            if (!row.HasCorrectionEMS) continue;
            var title = corrections[row.ActionActPlanId!.Value];
            if (title == row.Correction) continue;
            row.Correction = title;
            var plan = FindCorrectionPlan(row.ActionActPlanId);
            plan.Correction = title;
            changed.Add(plan);
        }
        if (changed.Count == 0) return;
        _actionActPlanRepository.CommitChanges();
        foreach (var plan in changed)
        {
            try { PropagateCorrection(plan); _actionActPlanRepository.CommitChanges(); }
            catch (System.Exception) { /* Java logs and continues with remaining plans. */ }
        }
    }
}
