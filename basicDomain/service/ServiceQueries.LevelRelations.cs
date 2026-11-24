#nullable enable
using Microsoft.EntityFrameworkCore;
using global::Common.utils;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ServiceQueries
{
    public async Task<ServicePage<PlaningLevelOrganizationDto>> LevelOrganizationsAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct)
    {
        var query = db.PlaningLevelOrganizations.AsNoTracking().AsQueryable();
        if (filterByLevel) query = query.Where(x => x.LevelId == levelId);
        if (filter is not null)
        {
            var pattern = $"%{filter}%";
            query = query.Where(x => x.Level != null && (EF.Functions.Like(x.Level.Code!, pattern)
                    || EF.Functions.Like(x.Level.DesLevel!, pattern)
                    || EF.Functions.Like(x.Level.TypeLevel.ToString(), pattern)
                    || EF.Functions.Like(x.Level.CreateDate.ToString()!, pattern))
                || x.Organization != null && (EF.Functions.Like(x.Organization.DesOrganE!, pattern)
                    || EF.Functions.Like(x.Organization.DesOrganF!, pattern)
                    || EF.Functions.Like(x.Organization.DesThreeChar!, pattern)
                    || EF.Functions.Like(x.Organization.ParentCode!, pattern)));
        }
        var projected = query.Select(x => new PlaningLevelOrganizationDto
        {
            Id = x.Id, DummyField = x.DummyField, LevelId = x.LevelId,
            DesLevel = x.Level == null ? null : x.Level.DesLevel,
            OrganizationId = x.OrganizationId, DesOrganF = x.Organization == null ? null : x.Organization.DesOrganF,
            ModirId = filterByLevel && x.Organization != null ? x.Organization.ModirId : null,
            ModirName = filterByLevel && x.Organization != null && x.Organization.Modir != null
                ? (x.Organization.Modir.NameF ?? "") + " " + (x.Organization.Modir.FamilyF ?? "") : null
        });
        return await PageProjectedAsync(projected, page, ct);
    }

    public async Task<ServicePage<PlaningLevelTeamDto>> LevelTeamsAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct)
    {
        var query = from team in db.PlaningLevelTeams.AsNoTracking()
                    join position in db.Set<Position>() on team.User!.PositionId equals (int?)position.Id into positions
                    from position in positions.DefaultIfEmpty()
                    join organization in db.Organizations on team.User!.OrganizationId equals (int?)organization.Id into organizations
                    from organization in organizations.DefaultIfEmpty()
                    select new { team, position, organization };
        if (filterByLevel && levelId.HasValue) query = query.Where(x => x.team.LevelId == levelId);
        if (filter is not null)
        {
            var pattern = $"%{filter}%";
            query = query.Where(x => filterByLevel
                ? EF.Functions.Like(x.team.Id.ToString(), pattern)
                    || EF.Functions.Like(x.team.LevelId.ToString()!, pattern)
                    || EF.Functions.Like(x.team.UserId.ToString()!, pattern)
                    || x.team.User != null && (EF.Functions.Like(x.team.User.NameF!, pattern) || EF.Functions.Like(x.team.User.FamilyF!, pattern))
                    || x.position != null && EF.Functions.Like(x.position.DesPost!, pattern)
                    || x.organization != null && EF.Functions.Like(x.organization.DesOrganF!, pattern)
                : x.team.Level != null && (EF.Functions.Like(x.team.Level.Code!, pattern)
                    || EF.Functions.Like(x.team.Level.DesLevel!, pattern)
                    || EF.Functions.Like(x.team.Level.TypeLevel.ToString(), pattern)
                    || EF.Functions.Like(x.team.Level.CreateDate.ToString()!, pattern))
                    || x.team.User != null && (EF.Functions.Like(x.team.User.NameF!, pattern) || EF.Functions.Like(x.team.User.FamilyF!, pattern)));
        }
        var projected = query.Select(x => new PlaningLevelTeamDto
        {
            Id = x.team.Id, LevelId = x.team.LevelId, UserId = x.team.UserId,
            NameF = x.team.User == null ? null : x.team.User.NameF,
            FamilyF = x.team.User == null ? null : x.team.User.FamilyF,
            DesPost = x.position == null ? null : x.position.DesPost,
            DesOrganF = x.organization == null ? null : x.organization.DesOrganF
        });
        return await PageProjectedAsync(projected, page, ct);
    }

    private static async Task<ServicePage<T>> PageProjectedAsync<T>(IQueryable<T> query, ServicePageRequest page, CancellationToken ct)
    {
        var request = ToPageRequest(page);
        var count = await query.LongCountAsync(ct);
        var items = await query.Apply(request).ToListAsync(ct);
        return new ServicePage<T>(items, count, request.Page, request.Size);
    }
}
