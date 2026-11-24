using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.service.impl;

public sealed class MeasureRepositoryEditService(ServiceRuntime runtime, IServiceProvider services) : ServiceBase<StrategyObject>(runtime)
{
    public Task<MeasureRepositoryDto> EditAsync(MeasureRepositoryDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Id is not long repositoryId || dto.MeasureCatalogDto?.Id is not long catalogId ||
            string.IsNullOrWhiteSpace(dto.MeasureName) || dto.MeasureConfirmStatus is not long repositoryStatus ||
            dto.MeasureCatalogDto.MeasureConfirmStatus is not long catalogStatus ||
            dto.MeasureCatalogDto.MeasurePeriod is not int period || dto.MeasureUnitId <= 0 ||
            repositoryStatus is < 0 or > 255 || catalogStatus is < 0 or > 255 ||
            dto.DesiredMethod is < 0 or > 255 || period is < 0 or > 255)
            throw new ArgumentException("اطلاعات مخزن، کاتالوگ، واحد یا وضعیت شاخص معتبر نیست.");

        return R.TransactionAsync(async () =>
        {
            var catalog = dto.MeasureCatalogDto;
            var stored = await R.Db.Database.SqlQuery<MeasureEditCatalogRow>($"""
                SELECT CAST(c.LevelId AS bigint) AS LevelId, CAST(c.Tashilgar AS bigint) AS TashilgarId
                FROM dbo.tblDimMeasureCatalog c WITH (UPDLOCK, HOLDLOCK)
                JOIN dbo.tblDimMeasureReporsitory r WITH (UPDLOCK, HOLDLOCK)
                  ON r.DimMeasureRepositoryId = c.DimMeasureCatalogRepositoryId
                WHERE c.DimMeasureCatalogId = {catalogId} AND r.DimMeasureRepositoryId = {repositoryId}
                """).SingleOrDefaultAsync(ct) ?? throw new ArgumentException("مخزن یا کاتالوگ شاخص یافت نشد.");
            await AccessAsync(stored.LevelId, ct);
            var levelId = catalog.PlaningLevelId ?? stored.LevelId;
            if (levelId != stored.LevelId)
                await MoveToLevelAsync(catalogId, repositoryId, levelId, ct);
            if (!await R.Db.Set<MeasureUnit>().AnyAsync(unit => unit.Id == dto.MeasureUnitId, ct))
                throw new ArgumentException("واحد اندازه‌گیری یافت نشد.");
            foreach (var userId in new[] { catalog.ModirId, catalog.TashilgarId, catalog.TashilgarHamkarId }.Where(id => id.HasValue).Select(id => id!.Value).Distinct())
                await R.External.GetUserAsync(userId, ct);

            await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.tblDimMeasureReporsitory SET MeasureName = {dto.MeasureName},
                    DesiredMethod = {dto.DesiredMethod}, MeasureType = {dto.MeasureType},
                    ConfirmStatus = {repositoryStatus}, MeasurePayeshType = {dto.MeasurePayeshType}, MeasureUnitId = {dto.MeasureUnitId}
                WHERE DimMeasureRepositoryId = {repositoryId}
                """, ct);
            await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.tblDimMeasureCatalog SET WorstMin = {catalog.WorstMin}, TargetTheory = {catalog.TargetTheory},
                    ZeroTarget = {catalog.ZeroTarget}, ZeroInitial = {catalog.ZeroInitial}, DataType = {catalog.DataType},
                    MeasurePeriod = {period}, ParentId = {catalog.ParentId}, IsDeleted = {catalog.IsDeleted},
                    ConfirmStatus = {catalogStatus}, MeasureMaxValue = {catalog.MeasureMaxValue},
                    MeasureStatus = {catalog.MeasureStatus}, OriginId = {catalog.OriginId}, LevelId = {levelId},
                    Modir = COALESCE({catalog.ModirId}, Modir), Tashilgar = COALESCE({catalog.TashilgarId}, Tashilgar),
                    TashilgarHamkar = COALESCE({catalog.TashilgarHamkarId}, TashilgarHamkar)
                WHERE DimMeasureCatalogId = {catalogId}
                """, ct);
            if (catalog.ModirId.HasValue || catalog.TashilgarId.HasValue)
                await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.tblDimMeasure SET Modir = COALESCE({catalog.ModirId}, Modir),
                        Tashilgar = COALESCE({catalog.TashilgarId}, Tashilgar)
                    WHERE DimMeasureCatalogId = {catalogId} AND IsDeleted = 0
                    """, ct);
            if (dto.PlanChanged)
            {
                await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    DELETE evaluation FROM eval.tblEvaluationMeasure evaluation
                    JOIN dbo.tblDimMeasureActPlan planItem ON planItem.DimMeasureActPlanId = evaluation.DimMeasureActId
                    JOIN dbo.tblDimMeasure measure ON measure.MeasureId = planItem.DimMeasureId
                    WHERE measure.DimMeasureCatalogId = {catalogId}
                    """, ct);
                await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    DELETE planItem FROM dbo.tblDimMeasureActPlan planItem
                    JOIN dbo.tblDimMeasure measure ON measure.MeasureId = planItem.DimMeasureId
                    WHERE measure.DimMeasureCatalogId = {catalogId}
                    """, ct);
                await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.tblDimMeasure SET IsPlanned = 0 WHERE DimMeasureCatalogId = {catalogId}
                    """, ct);
            }
            if (catalog.ModirId is long modirId) await R.External.AddModirUserRoleAsync(modirId, ct);
            if ((catalog.TashilgarId ?? stored.TashilgarId) is long facilitatorId)
                await R.External.AddTashilgarUserRoleAsync(facilitatorId, ct);
            foreach (var userId in new[] { catalog.ModirId, catalog.TashilgarId, catalog.TashilgarHamkarId }.Where(id => id.HasValue).Select(id => id!.Value).Distinct())
                await R.External.EnsureLevelMembershipAsync(userId, levelId, ct);
            await R.Db.SaveChangesAsync(ct);
            return await LoadResultAsync(repositoryId, ct);
        }, ct);
    }

    private async Task MoveToLevelAsync(long catalogId, long repositoryId, long levelId, CancellationToken ct)
    {
        await AccessAsync(levelId, ct);
        var conflict = await R.Db.Database.SqlQuery<long>($"""
            SELECT COUNT_BIG(*) AS Value FROM dbo.tblDimMeasureCatalog
            WHERE DimMeasureCatalogRepositoryId = {repositoryId} AND LevelId = {levelId}
            """).SingleAsync(ct);
        if (conflict != 0) throw new ArgumentException("کاتالوگ شاخص در کمیته انتخاب‌شده وجود دارد.");
        var relations = await R.Db.Database.SqlQuery<long>($"""
            SELECT COUNT_BIG(*) AS Value FROM dbo.tblDimMeasureCatalogRelation
            WHERE MainMeasureCatId = {catalogId} OR RelatedMeasureCatId = {catalogId}
            """).SingleAsync(ct);
        if (relations != 0) throw new ArgumentException("امکان تغییر سطح برنامه‌ریزی برای شاخص دارای روابط محاسباتی وجود ندارد.");
        var level = await R.Db.Set<PlaningLevel>().FindAsync([levelId], ct)
            ?? throw new ArgumentException("کمیته انتخاب‌شده یافت نشد.");
        var items = await R.Db.Database.SqlQuery<MeasureMoveRow>($"""
            SELECT CAST(MeasureId AS bigint) AS Id, CAST(StrategyObjectId AS bigint) AS StrategyObjectId
            FROM dbo.tblDimMeasure WHERE DimMeasureCatalogId = {catalogId}
            """).ToListAsync(ct);
        var strategies = services.GetRequiredService<StrategyObjectImpl>();
        foreach (var item in items)
        {
            if (item.StrategyObjectId is not long strategyId)
                throw new ArgumentException("هدف یکی از شاخص‌های کمیته مشخص نیست.");
            var source = await strategies.FindByIdOrElseThrowAsync(strategyId, ct);
            var target = await strategies.FindStrategyObjectWithChangedPlanningLevelAsync(source, level, ct);
            await R.Db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.tblDimMeasure SET StrategyObjectId = {target.Id} WHERE MeasureId = {item.Id}
                """, ct);
        }
    }

    private async Task<MeasureRepositoryDto> LoadResultAsync(long repositoryId, CancellationToken ct)
    {
        var row = await R.Db.Database.SqlQuery<MeasureEditedRow>($"""
            SELECT CAST(r.DimMeasureRepositoryId AS bigint) AS Id, r.MeasureName,
                CAST(r.MeasureUnitId AS bigint) AS MeasureUnitId, u.DesMeasureF AS MeasureUnitDescription,
                CAST(r.DesiredMethod AS int) AS DesiredMethod, r.MeasureType,
                CAST(r.ConfirmStatus AS bigint) AS MeasureConfirmStatus, r.MeasurePayeshType, r.AddToSubLevels
            FROM dbo.tblDimMeasureReporsitory r JOIN dbo.tblMeasureUnit u ON u.MeasureUnitId = r.MeasureUnitId
            WHERE r.DimMeasureRepositoryId = {repositoryId}
            """).SingleAsync(ct);
        var catalogs = await R.Db.Database.SqlQuery<MeasureCatalogGridRow>($"""
            SELECT CAST(c.DimMeasureCatalogId AS bigint) AS Id, c.WorstMin, c.TargetTheory, c.ZeroTarget, c.ZeroInitial,
                c.DataType, CAST(c.MeasurePeriod AS int) AS MeasurePeriod, c.ParentId, c.IsDeleted,
                CAST(c.ConfirmStatus AS bigint) AS MeasureConfirmStatus, c.MeasureMaxValue, c.MeasureStatus, c.OriginId,
                CAST(c.LevelId AS bigint) AS PlaningLevelId, CAST(c.DimMeasureCatalogRepositoryId AS bigint) AS MeasureRepositoryId,
                CAST(c.Modir AS bigint) AS ModirId, CAST(c.Tashilgar AS bigint) AS TashilgarId,
                CAST(c.TashilgarHamkar AS bigint) AS TashilgarHamkarId, r.IsDeleted AS RepositoryDeleted
            FROM dbo.tblDimMeasureCatalog c JOIN dbo.tblDimMeasureReporsitory r ON r.DimMeasureRepositoryId = c.DimMeasureCatalogRepositoryId
            WHERE c.DimMeasureCatalogRepositoryId = {repositoryId}
            """).ToListAsync(ct);
        return new MeasureRepositoryDto { Id = row.Id, MeasureName = row.MeasureName, MeasureUnitId = row.MeasureUnitId,
            MeasureUnitDescription = row.MeasureUnitDescription, DesiredMethod = row.DesiredMethod, MeasureType = row.MeasureType,
            MeasureConfirmStatus = row.MeasureConfirmStatus, MeasurePayeshType = row.MeasurePayeshType, AddToSubLevels = row.AddToSubLevels,
            LevelIds = catalogs.Select(item => item.PlaningLevelId).Distinct().ToList(),
            MeasureCatalogDtos = catalogs.Select(item => item.ToDto()).ToList() };
    }
}

public sealed class MeasureEditCatalogRow { public long LevelId { get; set; } public long? TashilgarId { get; set; } }
public sealed class MeasureMoveRow { public long Id { get; set; } public long? StrategyObjectId { get; set; } }
public sealed class MeasureEditedRow
{
    public long Id { get; set; }
    public string? MeasureName { get; set; }
    public long MeasureUnitId { get; set; }
    public string? MeasureUnitDescription { get; set; }
    public int DesiredMethod { get; set; }
    public byte MeasureType { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public byte MeasurePayeshType { get; set; }
    public bool AddToSubLevels { get; set; }
}
