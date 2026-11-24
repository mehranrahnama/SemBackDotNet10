#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public interface IPlaningLevelOrganizationRepository : ISemRepository<PlaningLevelOrganization, long>
{
    Task<PageResult<PlaningLevelOrganizationDto>> FindAllPlaningLevelOrganizationsAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<PageResult<PlaningLevelOrganizationDto>> FindPlaningLevelOrganizationsByLevelIDAsync(PageRequest page, long levelId, string? filter, CancellationToken ct = default);
    Task<List<PlaningLevelOrganization>> FindByOrganizationIdAsync(long organizationId, CancellationToken ct = default);
}

public sealed class PlaningLevelOrganizationRepository(DbContext db)
    : SemRepository<PlaningLevelOrganization, long>(db), IPlaningLevelOrganizationRepository
{
    public Task<PageResult<PlaningLevelOrganizationDto>> FindAllPlaningLevelOrganizationsAsync(PageRequest page, string? filter, CancellationToken ct = default)
    {
        var pat = "%" + filter + "%";
        var q = Set.Where(x => filter == null || EF.Functions.Like(x.Level.Code, pat) || EF.Functions.Like(x.Level.DesLevel, pat) || EF.Functions.Like(x.Level.TypeLevel.ToString(), pat) || EF.Functions.Like(x.Level.CreateDate.ToString(), pat) || EF.Functions.Like(x.Organization.DesOrganE, pat) || EF.Functions.Like(x.Organization.DesOrganF, pat) || EF.Functions.Like(x.Organization.DesThreeChar, pat) || EF.Functions.Like(x.Organization.ParentCode, pat));
        var projectedQuery = q
     .OrderBy(x => x.Id)
     .Select(x => new PlaningLevelOrganizationDto
     {
         Id = x.Id,
         DummyField = x.DummyField,

         LevelId = x.LevelId,

         DesLevel = x.Level == null
             ? null
             : x.Level.DesLevel,

         OrganizationId = x.OrganizationId,

         DesOrganF = x.Organization == null
             ? null
             : x.Organization.DesOrganF
     });

        return PageAsync(projectedQuery, page, ct);
    }

    public Task<PageResult<PlaningLevelOrganizationDto>> FindPlaningLevelOrganizationsByLevelIDAsync(PageRequest page, long levelId, string? filter, CancellationToken ct = default)
    {
        var pat = "%" + filter + "%";
        var q = Set.Where(x => filter == null || EF.Functions.Like(x.Level.Code, pat) || EF.Functions.Like(x.Level.DesLevel, pat) || EF.Functions.Like(x.Level.TypeLevel.ToString(), pat) || EF.Functions.Like(x.Level.CreateDate.ToString(), pat) || EF.Functions.Like(x.Organization.DesOrganE, pat) || EF.Functions.Like(x.Organization.DesOrganF, pat) || EF.Functions.Like(x.Organization.DesThreeChar, pat) || EF.Functions.Like(x.Organization.ParentCode, pat));
        q = q.Where(x => x.LevelId == levelId);
        var projectedQuery = q
    .OrderBy(x => x.Id)
    .Select(x => new PlaningLevelOrganizationDto
    {
        Id = x.Id,
        DummyField = x.DummyField,

        LevelId = x.LevelId,

        DesLevel = x.Level == null
            ? null
            : x.Level.DesLevel,

        OrganizationId = x.OrganizationId,

        DesOrganF = x.Organization == null
            ? null
            : x.Organization.DesOrganF,

        ModirName =
            x.Organization == null ||
            x.Organization.Modir == null
                ? null
                : (x.Organization.Modir.NameF ?? "") + " " +
                  (x.Organization.Modir.FamilyF ?? ""),

        ModirId = x.Organization == null
            ? (long?)null
            : x.Organization.ModirId
    });

        return PageAsync(projectedQuery, page, ct);

    }

    public Task<List<PlaningLevelOrganization>> FindByOrganizationIdAsync(long organizationId, CancellationToken ct = default)
    {
        return Set.Where(x => x.Organization.Id == organizationId).ToListAsync(ct);
    }
}
