using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.service.impl;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public sealed record MeasureRepositoryPageInput(long? LevelId, long? MeasureConfirmStatus, int? MeasureType, string? Filter);

public sealed class MeasureRepositoryGridService(ServiceRuntime runtime, IHttpContextAccessor http) : ServiceBase<StrategyObject>(runtime)
{
    public async Task<ServicePage<MeasureRepositoryDto>> FindAsync(MeasureRepositoryPageInput input, ServicePageRequest page, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.LevelId is not long levelId) throw new ArgumentException("LevelId is required.", nameof(input));
        await AccessAsync(levelId, ct);
        var identity = http.HttpContext?.User.Identity as ClaimsIdentity;
        var roles = identity?.FindAll(identity.RoleClaimType).Select(claim => claim.Value).ToArray() ?? [];
        var holding = R.TenantName == "MSCHOLDING";
        long? facilitatorId = !holding && R.TenantName == "HOSCO" && roles.Length > 0 && roles.All(role => role == "Tashilgar") ? R.User.Id : null;
        var measureType = input.MeasureType ?? 0;
        var filter = input.Filter?.Trim();
        var query = R.Db.Database.SqlQuery<MeasureRepositoryGridRow>($"""
            SELECT CAST(repository.DimMeasureRepositoryId AS bigint) AS Id,
                CAST(catalog.DimMeasureCatalogId AS bigint) AS CatalogId, catalog.code AS Code,
                repository.MeasureName, level.DesThreeChar, level.DesLevel AS PlaningLevelDesLevel,
                CAST(level.LevelId AS bigint) AS PlaningLevelId, unit.DesMeasureF AS MeasureUnitDescription,
                catalog.DataType AS CatalogDataType, CAST(repository.DesiredMethod AS int) AS DesiredMethod,
                repository.MeasureType, CAST(repository.ConfirmStatus AS bigint) AS MeasureConfirmStatus,
                CAST(catalog.ConfirmStatus AS bigint) AS MeasureConfirmStatusMC,
                repository.MeasurePayeshType, CAST(catalog.MeasurePeriod AS int) AS CatalogMeasurePeriod,
                CAST(unit.MeasureUnitId AS bigint) AS MeasureUnitId, repository.AddToSubLevels,
                repository.Formula, repository.FormulaDes,
                CONCAT(facilitator.NameF, ' ', facilitator.FamilyF) AS Tashilgar,
                CASE WHEN {holding} = 1 THEN NULL ELSE CONCAT(assistant.NameF, ' ', assistant.FamilyF) END AS TashilgarHamkar,
                CONCAT(responsible.NameF, ' ', responsible.FamilyF) AS Modir, catalog.IsDeleted AS MeasureCatalogDeleted
            FROM dbo.tblDimMeasureCatalog catalog
            JOIN dbo.tblPlaningLevel level ON level.LevelId = catalog.LevelId
            JOIN dbo.tblDimMeasureReporsitory repository ON repository.DimMeasureRepositoryId = catalog.DimMeasureCatalogRepositoryId
            JOIN dbo.tblMeasureUnit unit ON unit.MeasureUnitId = repository.MeasureUnitId
            LEFT JOIN dbo.tblUser facilitator ON facilitator.UserId = catalog.Tashilgar
            LEFT JOIN dbo.tblUser assistant ON assistant.UserId = catalog.TashilgarHamkar
            LEFT JOIN dbo.tblUser responsible ON responsible.UserId = catalog.Modir
            WHERE catalog.IsDeleted = 0 AND level.LevelId = {levelId}
              AND ({holding} = 0 OR {measureType} = 0 OR repository.MeasureType = {measureType})
              AND ({input.MeasureConfirmStatus} IS NULL OR catalog.ConfirmStatus = {input.MeasureConfirmStatus})
              AND ({facilitatorId} IS NULL OR catalog.Tashilgar = {facilitatorId} OR catalog.TashilgarHamkar = {facilitatorId})
              AND ({filter} IS NULL OR repository.MeasureName LIKE '%' + {filter} + '%'
                OR CONCAT(facilitator.NameF, ' ', facilitator.FamilyF) LIKE '%' + {filter} + '%'
                OR CONCAT(facilitator.FamilyF, ' ', facilitator.NameF) LIKE '%' + {filter} + '%'
                OR facilitator.NameF LIKE '%' + {filter} + '%' OR facilitator.FamilyF LIKE '%' + {filter} + '%'
                OR CONCAT(responsible.NameF, ' ', responsible.FamilyF) LIKE '%' + {filter} + '%'
                OR CONCAT(responsible.FamilyF, ' ', responsible.NameF) LIKE '%' + {filter} + '%'
                OR responsible.NameF LIKE '%' + {filter} + '%' OR responsible.FamilyF LIKE '%' + {filter} + '%'
                OR unit.DesMeasureF LIKE '%' + {filter} + '%'
                OR ({holding} = 0 AND (CONCAT(assistant.NameF, ' ', assistant.FamilyF) LIKE '%' + {filter} + '%'
                    OR CONCAT(assistant.FamilyF, ' ', assistant.NameF) LIKE '%' + {filter} + '%'
                    OR assistant.NameF LIKE '%' + {filter} + '%' OR assistant.FamilyF LIKE '%' + {filter} + '%')))
            """);
        var result = await PageAsync(query, page, ct);
        var ids = string.Join(',', result.Content.Select(row => row.Id).Distinct());
        var catalogs = ids.Length == 0 ? [] : await R.Db.Database.SqlQuery<MeasureCatalogGridRow>($"""
            SELECT CAST(c.DimMeasureCatalogId AS bigint) AS Id, c.WorstMin, c.TargetTheory, c.ZeroTarget, c.ZeroInitial,
                c.DataType, CAST(c.MeasurePeriod AS int) AS MeasurePeriod, c.ParentId, c.IsDeleted,
                CAST(c.ConfirmStatus AS bigint) AS MeasureConfirmStatus, c.MeasureMaxValue, c.MeasureStatus, c.OriginId,
                CAST(c.LevelId AS bigint) AS PlaningLevelId, CAST(c.DimMeasureCatalogRepositoryId AS bigint) AS MeasureRepositoryId,
                CAST(c.Modir AS bigint) AS ModirId, CAST(c.Tashilgar AS bigint) AS TashilgarId,
                CAST(c.TashilgarHamkar AS bigint) AS TashilgarHamkarId, repository.IsDeleted AS RepositoryDeleted
            FROM dbo.tblDimMeasureCatalog c
            JOIN dbo.tblDimMeasureReporsitory repository ON repository.DimMeasureRepositoryId = c.DimMeasureCatalogRepositoryId
            WHERE c.IsDeleted = 0 AND c.DimMeasureCatalogRepositoryId IN
                (SELECT TRY_CONVERT(int, value) FROM STRING_SPLIT({ids}, ','))
            """).ToListAsync(ct);
        var items = result.Content.Select(row => row.ToDto(catalogs, levelId)).ToList();
        return new ServicePage<MeasureRepositoryDto>(items, result.TotalElements, result.CurrentPage, result.PageSize);
    }
}

public sealed class MeasureRepositoryGridRow
{
    public long Id { get; set; }
    public long CatalogId { get; set; }
    public string? Code { get; set; }
    public string? MeasureName { get; set; }
    public string? DesThreeChar { get; set; }
    public string? PlaningLevelDesLevel { get; set; }
    public long PlaningLevelId { get; set; }
    public string? MeasureUnitDescription { get; set; }
    public byte? CatalogDataType { get; set; }
    public int DesiredMethod { get; set; }
    public byte MeasureType { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public long? MeasureConfirmStatusMC { get; set; }
    public byte MeasurePayeshType { get; set; }
    public int CatalogMeasurePeriod { get; set; }
    public long MeasureUnitId { get; set; }
    public bool AddToSubLevels { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
    public string? Tashilgar { get; set; }
    public string? TashilgarHamkar { get; set; }
    public string? Modir { get; set; }
    public bool MeasureCatalogDeleted { get; set; }

    public MeasureRepositoryDto ToDto(List<MeasureCatalogGridRow> allCatalogs, long levelId)
    {
        var catalogRows = allCatalogs.Where(catalog => catalog.MeasureRepositoryId == Id).ToList();
        var catalogDtos = catalogRows.Select(catalog => catalog.ToDto()).ToList();
        return new MeasureRepositoryDto
        {
            Id = Id, CatalogId = CatalogId, Code = Code is not null && DesThreeChar is not null && !Code.Contains(DesThreeChar) ? Code + "-" + DesThreeChar : Code,
            MeasureName = MeasureName, DesThreeChar = DesThreeChar, PlaningLevelDesLevel = PlaningLevelDesLevel,
            PlaningLevelId = PlaningLevelId, MeasureUnitDescription = MeasureUnitDescription, CatalogDataType = CatalogDataType ?? 0,
            DesiredMethod = DesiredMethod, MeasureType = MeasureType, MeasureConfirmStatus = MeasureConfirmStatus, MeasureConfirmStatusMC = MeasureConfirmStatusMC,
            MeasurePayeshType = MeasurePayeshType, CatalogMeasurePeriod = CatalogMeasurePeriod, MeasureUnitId = MeasureUnitId,
            AddToSubLevels = AddToSubLevels, Formula = Formula, FormulaDes = FormulaDes, Tashilgar = Tashilgar,
            TashilgarHamkar = TashilgarHamkar, Modir = Modir, MeasureCatalogDeleted = MeasureCatalogDeleted,
            MeasureCatalogDtos = catalogDtos, MeasureCatalogDto = catalogDtos.FirstOrDefault(catalog => catalog.PlaningLevelId == levelId) ?? new(),
            LevelIds = catalogRows.Where(catalog => !catalog.RepositoryDeleted).Select(catalog => catalog.PlaningLevelId).Distinct().ToList()
        };
    }
}

public sealed class MeasureCatalogGridRow
{
    public long Id { get; set; }
    public decimal? WorstMin { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool ZeroInitial { get; set; }
    public byte? DataType { get; set; }
    public int MeasurePeriod { get; set; }
    public int? ParentId { get; set; }
    public bool IsDeleted { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public decimal? MeasureMaxValue { get; set; }
    public int? MeasureStatus { get; set; }
    public int? OriginId { get; set; }
    public long PlaningLevelId { get; set; }
    public long MeasureRepositoryId { get; set; }
    public long? ModirId { get; set; }
    public long? TashilgarId { get; set; }
    public long? TashilgarHamkarId { get; set; }
    public bool RepositoryDeleted { get; set; }
    public MeasureCatalogDto ToDto() => new()
    {
        Id = Id, WorstMin = WorstMin, TargetTheory = TargetTheory, ZeroTarget = ZeroTarget, ZeroInitial = ZeroInitial,
        DataType = DataType, MeasurePeriod = MeasurePeriod, ParentId = ParentId, IsDeleted = IsDeleted,
        MeasureConfirmStatus = MeasureConfirmStatus, MeasureMaxValue = MeasureMaxValue, MeasureStatus = MeasureStatus,
        OriginId = OriginId, PlaningLevelId = PlaningLevelId, MeasureRepositoryId = MeasureRepositoryId,
        ModirId = ModirId, TashilgarId = TashilgarId, TashilgarHamkarId = TashilgarHamkarId
    };
}
