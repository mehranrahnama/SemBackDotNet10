#nullable enable
using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Repositories;

namespace SemBackDotNet10.service.impl;

public sealed class PlaningLevelServiceImpl(ServiceRuntime runtime, SemBackDotNet10.basicDomain.Repositories.IPlaningLevelRepository levels) : ServiceBase<PlaningLevel>(runtime)
{
    public Task<PlaningLevel> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default) => RequireAsync<PlaningLevel>(id, "error.planingLevel.idIsNotValid", ct);
    public Task<ServicePage<PlaningLevelDto>> FindAllAsync(string? filter, ServicePageRequest page, CancellationToken ct = default) => R.Queries.LevelsAsync(filter, page, ct);
    public async Task<ServicePage<PlaningLevelDto>> GetCurrentUserLevelsAsync(string? filter, string source, ServicePageRequest page, CancellationToken ct = default)
        => await R.Queries.CurrentUserLevelsAsync(filter, await AllowedLevelsAsync(source, ct), null, false, page, ct);
    public async Task<ServicePage<PlaningLevelDto>> GetCurrentUserLevelsByParentLevelIdAsync(string? filter, string source, long? parentId, ServicePageRequest page, CancellationToken ct = default)
        => await R.Queries.CurrentUserLevelsAsync(filter, await AllowedLevelsAsync(source == "evaluation" ? "object" : source, ct), parentId, true, page, ct);
    private async Task<List<long>> AllowedLevelsAsync(string source, CancellationToken ct) => R.User.IsAdmin
        ? await FindAllLevelIdsAsync(ct) : await R.External.GetUserLevelIdsAsync(source is "measure" or "action" or "evaluation" ? source : "object", R.User.Id, ct);
    public Task<List<long>> FindAllLevelIdsAsync(CancellationToken ct = default) => levels.FindAllLevelIdsAsync(ct);
    public Task<List<long>> FindSubLevelAsync(long levelId, CancellationToken ct = default) => levels.FindSubLevelAsync(levelId, ct);
    public async Task<ServicePage<PlaningLevelDto>> GetSupremeCommitteeAsync(ServicePageRequest page, CancellationToken ct = default)
    {
        var direction = page.Direction switch
        {
            SortDirection.Ascending => SemBackDotNet10.Common.enums.SortDirection.Asc,
            SortDirection.Descending => SemBackDotNet10.Common.enums.SortDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(page), "Invalid sort direction.")
        };
        var request = global::Common.utils.PageUtils.GetInstance(page.Page, page.Size, page.OrderBy, direction);
        var result = await levels.FindSupremeCommitteePageAsync(request, ct);
        return new ServicePage<PlaningLevelDto>(result.Items, result.TotalElements, result.Page, result.Size);
    }
    public async Task<List<PlaningLevelDto>> GetPlaningLevelsBySelectedUserTreeAsync(long userId, CancellationToken ct = default) => BuildTree(await levels.GetPlaningLevelsBySelectedUserTreeAsync(userId, ct));
    public async Task<List<PlaningLevelDto>> GetPlaningLevelTreeAsync(CancellationToken ct = default)
    {
        var isAdmin = R.User.IsAdmin;
        var allowedIds = isAdmin ? new List<long>() : await AllowedLevelsAsync("object", ct);
        return BuildTree(await levels.GetPlaningLevelsTreeAsync(isAdmin, allowedIds, ct));
    }
    public static List<PlaningLevelDto> BuildTree(List<PlaningLevelDto> levels)
    {
        var nodes = levels.ToDictionary(x => x.Id);
        foreach (var node in levels)
        {
            var visited = new HashSet<long>(); var current = node;
            while (true)
            {
                if (!visited.Add(current.Id)) throw new BusinessException("چرخه در سلسله‌مراتب کمیته‌ها وجود دارد.");
                if (current.ParentId is not long parent || !nodes.TryGetValue(parent, out var next)) break;
                current = next;
            }
        }
        var roots = new List<PlaningLevelDto>();
        foreach (var node in levels) { node.Children = []; node.HasChildren = false; node.IsRoot = false; }
        foreach (var node in levels)
        {
            if (node.ParentId is long parentId && nodes.TryGetValue(parentId, out var parent)) { parent.Children.Add(node); parent.HasChildren = true; }
            else { node.ParentId = null; node.IsRoot = true; roots.Add(node); }
        }
        return roots;
    }
    public PlaningLevelDto MapToDto(PlaningLevel x) => MapLevelToDto(x);

    internal static PlaningLevelDto MapLevelToDto(PlaningLevel x) => new()
    {
        Id = x.Id, FactoryId = x.FactoryId, Code = x.Code, DesThreeChar = x.DesThreeChar,
        DesLevel = x.DesLevel, TypeLevel = x.TypeLevel, CreateDate = x.CreateDate, Operational = x.Operational,
        ParentId = x.Parent == null ? null : x.Parent.Id,
        ResponsibleId = x.Responsible?.UserId, ResponsibleNameF = x.Responsible?.NameF, ResponsibleFamilyF = x.Responsible?.FamilyF,
        FacilitatorId = x.Facilitator?.UserId, FacilitatorNameF = x.Facilitator?.NameF, FacilitatorFamilyF = x.Facilitator?.FamilyF
    };
    public async Task<PlaningLevel> MapToEntityAsync(PlaningLevelDto dto, CancellationToken ct = default)
    { var entity = new PlaningLevel(); await ApplyAsync(entity, dto, false, ct); return entity; }
    private async Task ApplyAsync(PlaningLevel x, PlaningLevelDto dto, bool edit, CancellationToken ct)
    {
        if (!edit) x.FactoryId = dto.FactoryId;
        if (!edit || dto.Code != null) x.Code = dto.Code;
        if (!edit || dto.CreateDate != null) x.CreateDate = dto.CreateDate;
        x.DesThreeChar = dto.DesThreeChar;
        x.DesLevel = dto.DesLevel; x.TypeLevel = dto.TypeLevel; x.Operational = dto.Operational;
        if (!edit || dto.ParentId != null) x.Parent = dto.ParentId is long p ? await FindByIdOrElseThrowAsync(p, ct) : null;
        if (!edit || dto.FacilitatorId != null) x.Facilitator = dto.FacilitatorId is long f ? await R.External.GetUserAsync(f, ct) : null;
        if (!edit || dto.ResponsibleId != null) x.Responsible = dto.ResponsibleId is long r ? await R.External.GetUserAsync(r, ct) : null;
        if (!edit || dto.ModirId != null) x.Modir = dto.ModirId is long m ? await R.External.GetUserAsync(m, ct) : null;
        if (!edit || dto.TashilgarId != null) x.Tashilgar = dto.TashilgarId is long t ? await R.External.GetUserAsync(t, ct) : null;
        if (edit && x.Parent != null && (await FindUpperLevelIdsAsync(x.Parent, ct)).Any(p => p.Id == x.Id)) throw new BusinessException("کمیته نمی‌تواند فرزند خود باشد.");
    }
    private async Task LoadPeopleAsync(PlaningLevel x, CancellationToken ct)
    {
        await R.Db.Entry(x).Reference(e => e.Facilitator).LoadAsync(ct); await R.Db.Entry(x).Reference(e => e.Responsible).LoadAsync(ct);
        await R.Db.Entry(x).Reference(e => e.Modir).LoadAsync(ct); await R.Db.Entry(x).Reference(e => e.Tashilgar).LoadAsync(ct);
        await R.Db.Entry(x).Reference(e => e.Parent).LoadAsync(ct);
    }
    public Task<PlaningLevel> CreateAsync(PlaningLevelDto dto, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.ParentId == null && await GetRepository().AnyAsync(x => x.Parent == null, ct)) throw new BusinessException("امکان ایجاد بیش از یک کمیته بدون پدر وجود ندارد.");
        var x = await MapToEntityAsync(dto, ct); await SaveAsync(x, ct);
        var active = await R.External.FindActivePlanPeriodAsync(ct);
        if (active != null)
        {
            long order = 1;
            foreach (var title in new[] { "ارزش", "مشتری", "فرآیندهای داخلی", "توانمندسازها" })
                R.Db.Set<Perspective>().Add(new Perspective { DesperspectF = title, Orders = order++, PlaningLevel = x, PlanPeriod = active });
        }
        await AddRolesAsync(x, ct); return x;
    }, ct);
    public Task<PlaningLevel> EditAsync(PlaningLevelDto dto, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        ArgumentNullException.ThrowIfNull(dto); await AccessAsync(dto.Id, ct);
        var x = await FindByIdOrElseThrowAsync(dto.Id, ct); await LoadPeopleAsync(x, ct);
        long? oldF = x.Facilitator?.UserId, oldR = x.Responsible?.UserId, oldM = x.Modir?.UserId, oldT = x.Tashilgar?.UserId;
        await ApplyAsync(x, dto, true, ct);
        if (R.TenantName == "HOSCO") await R.External.ChangeFacilitatorAsync(x, dto.TashilgarId, oldT, dto.ModirId, oldM, ct);
        if (R.TenantName == "SEFIDDASHT") await R.External.ChangeFacilitatorAsync(x, dto.FacilitatorId, oldF, dto.ResponsibleId, oldR, ct);
        await SaveAsync(x, ct); await AddRolesAsync(x, ct);
        if (R.TenantName != "MSCHOLDING") await RemoveUnusedRolesAsync(oldF, oldR, ct);
        return x;
    }, ct);
    private async Task AddRolesAsync(PlaningLevel x, CancellationToken ct)
    {
        if (x.Facilitator != null) await R.External.AddDabirComiteRoleAsync(x.Facilitator.UserId, ct);
        if (x.Responsible != null) await R.External.AddRaeesComiteAsync(x.Responsible.UserId, R.TenantName, ct);
        if (x.Modir != null) await R.External.AddModirUserRoleAsync(x.Modir.UserId, ct);
        if (x.Tashilgar != null) await R.External.AddTashilgarUserRoleAsync(x.Tashilgar.UserId, ct);
    }
    private async Task RemoveUnusedRolesAsync(long? f, long? r, CancellationToken ct)
    {
        if (r is long responsible && !await GetRepository().AnyAsync(x => x.Responsible != null && x.Responsible.UserId == responsible, ct)) await R.External.RemoveRaeesComiteAsync(responsible, ct);
        if (f is long facilitator && !await GetRepository().AnyAsync(x => x.Facilitator != null && x.Facilitator.UserId == facilitator, ct)) await R.External.RemoveDabirComiteRoleAsync(facilitator, ct);
    }
    public Task DeleteAsync(long id, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        await AccessAsync(id, ct); var x = await FindByIdOrElseThrowAsync(id, ct); await LoadPeopleAsync(x, ct);
        long? f = x.Facilitator?.UserId, r = x.Responsible?.UserId;
        await base.DeleteAsync(x, ct); await RemoveUnusedRolesAsync(f, r, ct);
    }, ct);
    public async Task<long?> GetFacilitatorIdOfCommitteeAsync(long id, CancellationToken ct = default)
    { await AccessAsync(id, ct); if (R.TenantName == "MSCHOLDING") return null; var x = await FindByIdOrElseThrowAsync(id, ct); await LoadPeopleAsync(x, ct); return x.Facilitator?.UserId; }
    public async Task<long?> GetResponsibleIdOfCommitteeAsync(long id, CancellationToken ct = default)
    { await AccessAsync(id, ct); if (R.TenantName == "MSCHOLDING") return null; var x = await FindByIdOrElseThrowAsync(id, ct); await LoadPeopleAsync(x, ct); return x.Responsible?.UserId; }
    public async Task<List<PlaningLevel>> FindUpperLevelIdsAsync(PlaningLevel? level, CancellationToken ct = default)
    {
        var result = new List<PlaningLevel>(); var visited = new HashSet<long>();
        while (level != null)
        {
            if (!visited.Add(level.Id)) throw new BusinessException("چرخه در سلسله‌مراتب کمیته‌ها وجود دارد.");
            result.Add(level); await R.Db.Entry(level).Reference(x => x.Parent).LoadAsync(ct); level = level.Parent;
        }
        return result;
    }
    public Task InsertNewCommitteeTahavolAsync(CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        if (!R.User.IsAdmin) throw new UnauthorizedAccessException();
        foreach (var item in await R.External.GetCommitteeTahavolAsync(ct))
        {
            var level = await GetRepository().SingleOrDefaultAsync(x => x.Code == item.Code, ct);
            if (level == null)
            {
                var root = await GetRepository().SingleOrDefaultAsync(x => x.Parent == null, ct) ?? throw new BusinessException("کمیته ریشه یافت نشد.");
                level = new PlaningLevel { Code = item.Code, DesLevel = item.Name, TypeLevel = 2, Parent = root };
                var f = item.Members.LastOrDefault(x => x.Role == 1); var r = item.Members.LastOrDefault(x => x.Role == 2);
                if (f != null) level.Facilitator = await R.External.GetUserAsync(f.PersonalCode, ct);
                if (r != null) level.Responsible = await R.External.GetUserAsync(r.PersonalCode, ct);
                await SaveAsync(level, ct);
                if (f != null) { await R.External.AddDabirComiteRoleAsync(f.PersonalCode, ct); await R.External.EnsureLevelMembershipAsync(f.PersonalCode, level.Id, ct); }
                if (r != null) { await R.External.AddRaeesComiteAsync(r.PersonalCode, "HOSCO", ct); await R.External.EnsureLevelMembershipAsync(r.PersonalCode, level.Id, ct); }
            }
            foreach (var unit in item.Units)
                if (!await R.Db.Set<PlaningLevelUnit>().AnyAsync(x => x.UnitCode == unit.Code, ct))
                    R.Db.Set<PlaningLevelUnit>().Add(new PlaningLevelUnit { PlaningLevel = level, UnitCode = unit.Code, UnitTitle = unit.Name });
            await R.Db.SaveChangesAsync(ct);
        }
    }, ct);
}
