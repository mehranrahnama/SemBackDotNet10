using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.service.impl;

public sealed class ActionCatalogPageService(ServiceRuntime runtime, IHttpContextAccessor http) : ServiceBase<ActionPlanCatalog>(runtime)
{
    public async Task<object> ReadAsync(string filter, long? levelId, long? status, ServicePageRequest? page, CancellationToken ct, string? planYear = null, long? tenantId = null)
    {
        await AccessAsync(levelId, ct);
        var identity = http.HttpContext?.User.Identity as ClaimsIdentity;
        var roles = identity?.FindAll(identity.RoleClaimType).Select(c => c.Value).ToArray() ?? [];
        long? facilitator = R.TenantName == "HOSCO" && roles.Length > 0 && roles.All(x => x == "Tashilgar") ? R.User.Id : null;
        var pattern = "%" + filter + "%";
        var query = R.Db.Set<ActionPlanCatalog>().IgnoreQueryFilters().AsNoTracking()
            .Where(x => (planYear != null || x.Confirmstatus != 50) && levelId.HasValue && x.PlaningLevel != null && x.PlaningLevel.Id == levelId
                && (!status.HasValue || x.Confirmstatus == status)
                && (!tenantId.HasValue || x.PlaningLevel.FactoryId == tenantId.Value)
                && (planYear == null || string.Compare(x.EndDate.Substring(0, 4), planYear) >= 0)
                && (!facilitator.HasValue || (x.Payeshgar != null && x.Payeshgar.UserId == facilitator))
                && (EF.Functions.Like(x.Description!, pattern) || EF.Functions.Like(x.StartDate, pattern)
                    || EF.Functions.Like(x.EndDate, pattern) || EF.Functions.Like(x.PlaningLevel.DesLevel!, pattern)))
            .Select(x => new ActionPlanCatalogDto
            {
                // Normalize integer widths in SQL before SqlClient reads the DTO columns.
                Id = Convert.ToInt64(x.Id), Code = x.Code ?? x.Id.ToString(), Description = x.Description,
                StartDate = x.StartDate, EndDate = x.EndDate, TotalBudget = x.TotalBudget,
                ActionConfirmStatus = Convert.ToInt64(x.Confirmstatus), LevelId = Convert.ToInt64(x.PlaningLevel!.Id),
                DesLevel = x.PlaningLevel.DesLevel, DesThreeChar = x.PlaningLevel.DesThreeChar,
                CreateDate = x.Createddate, DeleteStatus = x.DeleteStatus,
                PayeshgarId = x.Payeshgar == null ? null : Convert.ToInt64(x.Payeshgar.UserId),
                ModirId = x.Modir == null ? null : Convert.ToInt64(x.Modir.UserId),
                Payeshgar = (x.Payeshgar!.NameF ?? "") + " " + (x.Payeshgar.FamilyF ?? ""),
                Modir = (x.Modir!.NameF ?? "") + " " + (x.Modir.FamilyF ?? "")
            });
        if (page is not null) return await PageAsync(query, page, ct);
        var rows = await query.ToListAsync(ct);
        return new { content = rows, totalElements = (long)rows.Count, totalPages = 1, currentPage = 0, pageSize = rows.Count };
    }
}
