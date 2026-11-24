using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SemBackDotNet10.basicDomain.Model.Dto;


namespace SemBackDotNet10.basicDomain.Repositories;

// ---------- PlaningLevel ----------

public interface IPlaningLevelRepository : IBaseRepository<PlaningLevel>
{
    Task<PlaningLevel?> FindByParentIsNullAsync(CancellationToken ct = default);

    Task<List<PlaningLevel>> FindParentLessPlaningLevelsAsync(CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelDto>> FindSupremeCommitteePageAsync(PageRequest page, CancellationToken ct = default);

    Task<List<long>> FindAllLevelIdsAsync(CancellationToken ct = default);

    Task<List<long>> GetAllCommitteeResponsiblesAsync(CancellationToken ct = default);

    Task<List<long>> GetAllCommitteeFacilitatorsAsync(CancellationToken ct = default);

    Task<List<long>> FindSubLevelAsync(long levelId, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelDto>> FindAllPlaningLevelsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelDto>> FindCurrentUserLevelsAsync(string keyword, List<long> levelIds, PageRequest pageRequest, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelDto>> FindCurrentUserLevelsByParentLevelIdAsync(string keyword, List<long> levelIds, long? parentLevelId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PlaningLevelDto>> GetPlaningLevelsBySelectedUserTreeAsync(long userId, CancellationToken ct = default);

    Task<List<PlaningLevelDto>> GetPlaningLevelsTreeAsync(bool isAdmin, List<long> userLevelIds, CancellationToken ct = default);
}

public class PlaningLevelRepository : BaseRepository<PlaningLevel>, IPlaningLevelRepository
{
    private readonly SemBackDotNet10.SecurityDomain.Tenancy.TenantContext tenant;
    public PlaningLevelRepository(SemDbContext context, SemBackDotNet10.SecurityDomain.Tenancy.TenantContext tenant) : base(context) => this.tenant = tenant;
    private IQueryable<PlaningLevel> TenantLevels => Context.PlaningLevels.Where(level => level.FactoryId == tenant.TenantId);

    public Task<PlaningLevel?> FindByParentIsNullAsync(CancellationToken ct = default) =>
        TenantLevels.FirstOrDefaultAsync(p => p.ParentId == null, ct);

    public Task<List<PlaningLevel>> FindParentLessPlaningLevelsAsync(CancellationToken ct = default) =>
        TenantLevels.Where(p => p.ParentId == null).ToListAsync(ct);

    public async Task<PlaningPagedResult<PlaningLevelDto>> FindSupremeCommitteePageAsync(PageRequest page, CancellationToken ct = default)
    {
        var query = TenantLevels.AsNoTracking().Where(p => p.ParentId == null);
        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(page).Select(p => new PlaningLevelDto { Id = p.Id, DesLevel = p.DesLevel }).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelDto> { Items = items, TotalElements = total, Page = page.Page, Size = page.Size };
    }

    public Task<List<long>> FindAllLevelIdsAsync(CancellationToken ct = default) =>
        TenantLevels.Select(p => p.Id).ToListAsync(ct);

    public Task<List<long>> GetAllCommitteeResponsiblesAsync(CancellationToken ct = default) =>
        TenantLevels.Where(p => p.ResponsibleId != null)
            .Select(p => p.ResponsibleId!.Value).ToListAsync(ct);

    public Task<List<long>> GetAllCommitteeFacilitatorsAsync(CancellationToken ct = default) =>
        TenantLevels.Where(p => p.FacilitatorId != null)
            .Select(p => p.FacilitatorId!.Value).ToListAsync(ct);

    public Task<List<long>> FindSubLevelAsync(long levelId, CancellationToken ct = default) =>
        TenantLevels.Where(p => p.ParentId == levelId).Select(p => p.Id).ToListAsync(ct);

    public async Task<PlaningPagedResult<PlaningLevelDto>> FindAllPlaningLevelsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default)
    {
        var source = TenantLevels.AsNoTracking();
        var query = FilterLevels(source, keyword, includeParent: true, includeShortCode: false);

        // The Java count query additionally searches the short code.
        var total = await FilterLevels(source, keyword, includeParent: true).LongCountAsync(ct);
        var items = await ProjectPage(query, pageRequest).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PlaningPagedResult<PlaningLevelDto>> FindCurrentUserLevelsAsync(string keyword, List<long> levelIds, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = FilterLevels(TenantLevels.AsNoTracking()
            .Where(p => levelIds.Contains(p.Id)), keyword, includeParent: false);

        var total = await query.LongCountAsync(ct);
        var items = await ProjectPage(query, pageRequest).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PlaningPagedResult<PlaningLevelDto>> FindCurrentUserLevelsByParentLevelIdAsync(string keyword, List<long> levelIds, long? parentLevelId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = FilterLevels(TenantLevels.AsNoTracking()
            .Where(p => parentLevelId != null && levelIds.Contains(p.Id) && p.ParentId == parentLevelId),
            keyword, includeParent: true);

        var total = await query.LongCountAsync(ct);
        var items = await ProjectPage(query, pageRequest, includeDetails: false).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<List<PlaningLevelDto>> GetPlaningLevelsBySelectedUserTreeAsync(long userId, CancellationToken ct = default)
    {
        return await TenantLevels.AsNoTracking()
            .Select(p => new PlaningLevelDto
            {
                Id = p.Id,
                ParentId = p.ParentId,
                DesLevel = p.DesLevel,
                IsChecked = Context.PlaningLevelUsers.Any(plu => plu.UserId == userId && plu.LevelId == p.Id)
            })
            .ToListAsync(ct);
    }

    public async Task<List<PlaningLevelDto>> GetPlaningLevelsTreeAsync(bool isAdmin, List<long> userLevelIds, CancellationToken ct = default)
    {
        var query = TenantLevels;
        if (!isAdmin)
            query = query.Where(p => userLevelIds.Contains(p.Id));

        return await query.Select(LevelProjection(includeDetails: true)).ToListAsync(ct);
    }

    private IQueryable<PlaningLevelDto> ProjectPage(IQueryable<PlaningLevel> query, PageRequest request, bool includeDetails = true)
    {
        if (string.IsNullOrWhiteSpace(request.OrderBy) || request.Direction is null)
        {
            query = query.OrderBy(p => p.Id);
            request = request with { OrderBy = null };
        }
        return query.Apply(request).Select(LevelProjection(includeDetails));
    }

    private static IQueryable<PlaningLevel> FilterLevels(IQueryable<PlaningLevel> query, string? keyword,
        bool includeParent, bool includeShortCode = true)
    {
        if (keyword is null) return query;
        var pattern = $"%{keyword}%";
        return query.Where(p => EF.Functions.Like(p.Code!, pattern)
            || EF.Functions.Like(p.DesLevel!, pattern)
            || (includeShortCode && EF.Functions.Like(p.DesThreeChar!, pattern))
            || EF.Functions.Like(p.TypeLevel.ToString(), pattern)
            || (p.CreateDate != null && EF.Functions.Like(p.CreateDate, pattern))
            || (p.Responsible != null && (EF.Functions.Like(p.Responsible.NameF!, pattern) || EF.Functions.Like(p.Responsible.FamilyF!, pattern)))
            || (p.Facilitator != null && (EF.Functions.Like(p.Facilitator.NameF!, pattern) || EF.Functions.Like(p.Facilitator.FamilyF!, pattern)))
            || (includeParent && p.Parent != null && EF.Functions.Like(p.Parent.DesLevel!, pattern)));
    }

    private Expression<Func<PlaningLevel, PlaningLevelDto>> LevelProjection(bool includeDetails) => p => new PlaningLevelDto()
    {
        Id = p.Id,
        Code = p.Code,
        DesLevel = p.DesLevel,
        DesThreeChar = p.DesThreeChar,
        TypeLevel = p.TypeLevel,
        CreateDate = p.CreateDate,
        FactoryId = p.FactoryId,
        ParentId = p.ParentId,
        ParentDesLevel = p.Parent != null ? p.Parent.DesLevel : null,
        ParentTypeLevel = includeDetails && p.Parent != null ? (short?)p.Parent.TypeLevel : null,
        Operational = includeDetails ? p.Operational : null,
        ModirId = includeDetails ? p.ModirId : null,
        ModirNameF = includeDetails && p.Modir != null ? p.Modir.NameF : null,
        ModirFamilyF = includeDetails && p.Modir != null ? p.Modir.FamilyF : null,
        TashilgarId = includeDetails ? p.TashilgarId : null,
        TashilgarNameF = includeDetails && p.Tashilgar != null ? p.Tashilgar.NameF : null,
        TashilgarFamilyF = includeDetails && p.Tashilgar != null ? p.Tashilgar.FamilyF : null,
        HasChildren = includeDetails && Context.PlaningLevels.Any(child => child.ParentId == p.Id && child.FactoryId == p.FactoryId),
        ResponsibleId = p.ResponsibleId,
        ResponsibleNameF = p.Responsible != null ? p.Responsible.NameF : null,
        ResponsibleFamilyF = p.Responsible != null ? p.Responsible.FamilyF : null,
        FacilitatorId = p.FacilitatorId,
        FacilitatorNameF = p.Facilitator != null ? p.Facilitator.NameF : null,
        FacilitatorFamilyF = p.Facilitator != null ? p.Facilitator.FamilyF : null
    };
}

// ---------- PlaningLevelUser ----------

public interface IPlaningLevelUserRepository : IBaseRepository<PlaningLevelUser>
{
    Task<List<long>> GetCurrentUserLevelIdsAsync(long userId, CancellationToken ct = default);

    Task<PlaningLevelUser?> FindByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default);

    Task<List<PlaningLevelUser>> FindByUserIdAsync(long userId, CancellationToken ct = default);

    Task<PlaningLevelUser?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default);

    Task<List<long>> FindLevelIdsByUserIdAsync(long userId, CancellationToken ct = default);

    Task<bool> ExistsByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default);

    Task<bool> CheckIsUserInPlanningLevelAsync(long levelId, long userId, CancellationToken ct = default);
}

public class PlaningLevelUserRepository : BaseRepository<PlaningLevelUser>, IPlaningLevelUserRepository
{
    public PlaningLevelUserRepository(SemDbContext context) : base(context) { }

    public Task<List<long>> GetCurrentUserLevelIdsAsync(long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers
            .Where(p => p.UserId == userId && p.LevelId.HasValue)
            .Select(p => p.LevelId!.Value)
            .ToListAsync(ct);

    public Task<PlaningLevelUser?> FindByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers.FirstOrDefaultAsync(p => p.LevelId == levelId && p.UserId == userId, ct);

    public Task<List<PlaningLevelUser>> FindByUserIdAsync(long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers.Where(p => p.UserId == userId).ToListAsync(ct);

    public Task<PlaningLevelUser?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers.FirstOrDefaultAsync(p => p.UserId == userId && p.LevelId == levelId, ct);

    public Task<List<long>> FindLevelIdsByUserIdAsync(long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers
            .Where(p => p.UserId == userId && p.LevelId.HasValue)
            .Select(p => p.LevelId!.Value)
            .ToListAsync(ct);

    public Task<bool> ExistsByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers.AnyAsync(p => p.LevelId == levelId && p.UserId == userId, ct);

    public Task<bool> CheckIsUserInPlanningLevelAsync(long levelId, long userId, CancellationToken ct = default) =>
        Context.PlaningLevelUsers.AnyAsync(p => p.LevelId == levelId && p.UserId == userId, ct);
}

// ---------- PlaningLevelTeam ----------

public interface IPlaningLevelTeamRepository : IBaseRepository<PlaningLevelTeam>
{
    Task<PlaningLevelTeam?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelTeamDto>> FindAllPlaningLevelTeamsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelTeamDto>> FindByLevelIdAsync(string keyword, long levelId, PageRequest pageRequest, CancellationToken ct = default);
}

public class PlaningLevelTeamRepository : BaseRepository<PlaningLevelTeam>, IPlaningLevelTeamRepository
{
    public PlaningLevelTeamRepository(SemDbContext context) : base(context) { }

    public Task<PlaningLevelTeam?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default) =>
        Context.PlaningLevelTeams.FirstOrDefaultAsync(t => t.UserId == userId && t.LevelId == levelId, ct);

    public async Task<PlaningPagedResult<PlaningLevelTeamDto>> FindAllPlaningLevelTeamsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevelTeams
            .Select(t => new PlaningLevelTeamDto { Id = t.Id, UserId = t.UserId, LevelId = t.LevelId });

        var total = await query.LongCountAsync(ct);
        var items = await query.OrderBy(x => x.Id)
            .Skip(PlaningPaging.GetOffset(pageRequest.Page, pageRequest.Size))
            .Take(pageRequest.Size).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelTeamDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PlaningPagedResult<PlaningLevelTeamDto>> FindByLevelIdAsync(string keyword, long levelId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevelTeams
            .Where(t => t.LevelId == levelId)
            .Select(t => new PlaningLevelTeamDto { Id = t.Id, UserId = t.UserId, LevelId = t.LevelId });

        var total = await query.LongCountAsync(ct);
        var items = await query.OrderBy(x => x.Id)
            .Skip(PlaningPaging.GetOffset(pageRequest.Page, pageRequest.Size))
            .Take(pageRequest.Size).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelTeamDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}

// ---------- PlaningLevelUnit ----------

public interface IPlaningLevelUnitRepository : IBaseRepository<PlaningLevelUnit>
{
    Task<PlaningLevelUnit?> FindByUnitCodeAsync(string unitCode, CancellationToken ct = default);
}

public class PlaningLevelUnitRepository : BaseRepository<PlaningLevelUnit>, IPlaningLevelUnitRepository
{
    public PlaningLevelUnitRepository(SemDbContext context) : base(context) { }

    public Task<PlaningLevelUnit?> FindByUnitCodeAsync(string unitCode, CancellationToken ct = default) =>
        Context.PlaningLevelUnits.FirstOrDefaultAsync(u => u.UnitCode == unitCode, ct);
}

// ---------- PlaningLevelOrganization ----------

public interface IPlaningLevelOrganizationRepository : IBaseRepository<PlaningLevelOrganization>
{
    Task<List<PlaningLevelOrganization>> FindByOrganizationIdAsync(long organizationId, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelOrganizationDto>> FindAllPlaningLevelOrganizationsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default);

    Task<PlaningPagedResult<PlaningLevelOrganizationDto>> FindPlaningLevelOrganizationsByLevelIdAsync(long levelId, string keyword, PageRequest pageRequest, CancellationToken ct = default);
}

public class PlaningLevelOrganizationRepository : BaseRepository<PlaningLevelOrganization>, IPlaningLevelOrganizationRepository
{
    public PlaningLevelOrganizationRepository(SemDbContext context) : base(context) { }

    public Task<List<PlaningLevelOrganization>> FindByOrganizationIdAsync(long organizationId, CancellationToken ct = default) =>
        Context.PlaningLevelOrganizations.Where(p => p.OrganizationId == organizationId).ToListAsync(ct);

    public async Task<PlaningPagedResult<PlaningLevelOrganizationDto>> FindAllPlaningLevelOrganizationsAsync(string keyword, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevelOrganizations
            .Select(p => new PlaningLevelOrganizationDto
            {
                Id = p.Id,
                DummyField = p.DummyField,
                LevelId = p.LevelId,
                OrganizationId = p.OrganizationId
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.OrderBy(x => x.Id)
            .Skip(PlaningPaging.GetOffset(pageRequest.Page, pageRequest.Size))
            .Take(pageRequest.Size).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelOrganizationDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PlaningPagedResult<PlaningLevelOrganizationDto>> FindPlaningLevelOrganizationsByLevelIdAsync(long levelId, string keyword, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevelOrganizations
            .Where(p => p.LevelId == levelId)
            .Select(p => new PlaningLevelOrganizationDto
            {
                Id = p.Id,
                DummyField = p.DummyField,
                LevelId = p.LevelId,
                OrganizationId = p.OrganizationId
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.OrderBy(x => x.Id)
            .Skip(PlaningPaging.GetOffset(pageRequest.Page, pageRequest.Size))
            .Take(pageRequest.Size).ToListAsync(ct);
        return new PlaningPagedResult<PlaningLevelOrganizationDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}

// ---------- PlanPeriod ----------

public interface IPlanPeriodRepository : IBaseRepository<PlanPeriod>
{
    Task<PlanPeriod?> FindByPlanIdAsync(long planId, CancellationToken ct = default);
    Task<PlanPeriod?> FindPlanPeriodByPlanIdAsync(long planId, CancellationToken ct = default);

    Task<PlanPeriod?> FindActivePlanPeriodAsync(CancellationToken ct = default);
}

public class PlanPeriodRepository : BaseRepository<PlanPeriod>, IPlanPeriodRepository
{
    public PlanPeriodRepository(SemDbContext context) : base(context) { }

    public Task<PlanPeriod?> FindByPlanIdAsync(long planId, CancellationToken ct = default) =>
        Context.PlanPeriods.FirstOrDefaultAsync(p => p.PlanId == planId, ct);

    public Task<PlanPeriod?> FindPlanPeriodByPlanIdAsync(long planId, CancellationToken ct = default) =>
        Context.PlanPeriods.SingleOrDefaultAsync(p => p.PlanId == planId, ct);

    public Task<PlanPeriod?> FindActivePlanPeriodAsync(CancellationToken ct = default) =>
        Context.PlanPeriods.FirstOrDefaultAsync(p => p.StatusType == 2, ct);
}
