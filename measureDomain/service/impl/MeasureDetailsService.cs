using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.service.impl;

public sealed class MeasureDetailsService(ServiceRuntime runtime)
{
    public async Task<MeasureDto?> FindByCatalogAsync(long planId, long catalogId, CancellationToken ct)
    {
        var row = await runtime.Db.Database.SqlQuery<MeasureDetailsRow>($"""
            SELECT CAST(m.measureid AS bigint) AS Id, o.title AS StrategyObjectTitle,
                CAST(o.strategyobjectid AS bigint) AS StrategyObjectId, r.measurename AS MeasureName,
                CONCAT(t.nameF, ' ', t.familyF) AS Tashilgar, CONCAT(manager.nameF, ' ', manager.familyF) AS Modir,
                CAST(c.measureperiod AS int) AS MeasurePeriod, unit.desmeasuref AS DesMeasureF,
                CAST(m.weight AS real) AS Weight, CAST(c.dimmeasurecatalogid AS bigint) AS MeasureCatalogId,
                m.currentvalue AS CurrentValue, m.currentyeartarget AS CurrentYearTarget,
                m.oneyearlatertarget AS OneYearLaterTarget, m.twoyearlatertarget AS TwoYearLaterTarget,
                c.datatype AS DataType, CAST(r.desiredmethod AS int) AS DesiredMethod,
                r.formula AS Formula, r.formulades AS FormulaDes, m.targettheory AS TargetTheory,
                m.zerotarget AS ZeroTarget, m.flgkpi AS FlgKpi, m.zeroinitial AS ZeroInitial,
                m.perfectextreme AS PerfectExtreme, m.warnextreme AS WarnExtreme,
                CAST(m.confirmstatus AS bigint) AS MeasureConfirmStatus, m.isdeleted AS Deleted
            FROM dbo.tbldimmeasure m
            LEFT JOIN dbo.tblstrategyobject o ON o.strategyobjectid = m.strategyobjectid
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblDimMeasureReporsitory r ON r.dimmeasurerepositoryid = c.dimmeasurecatalogrepositoryid
            JOIN dbo.tblmeasureunit unit ON unit.measureunitid = r.measureunitid
            LEFT JOIN dbo.tblUser t ON t.userid = c.tashilgar
            LEFT JOIN dbo.tblUser manager ON manager.userid = c.modir
            WHERE m.planid = {planId} AND m.dimmeasurecatalogid = {catalogId} AND m.isdeleted = 0
            """).SingleOrDefaultAsync(ct);
        return row is null ? null : new MeasureDto
        {
            Id = row.Id, StrategyObjectTitle = row.StrategyObjectTitle, StrategyObjectId = row.StrategyObjectId,
            MeasureName = row.MeasureName, Tashilgar = row.Tashilgar, Modir = row.Modir,
            MeasurePeriod = row.MeasurePeriod, DesMeasureF = row.DesMeasureF, Weight = row.Weight,
            MeasureCatalogId = row.MeasureCatalogId, CurrentValue = row.CurrentValue,
            CurrentYearTarget = row.CurrentYearTarget, OneYearLaterTarget = row.OneYearLaterTarget,
            TwoYearLaterTarget = row.TwoYearLaterTarget, DataType = row.DataType,
            DesiredMethod = row.DesiredMethod, Formula = row.Formula, FormulaDes = row.FormulaDes,
            TargetTheory = row.TargetTheory, ZeroTarget = row.ZeroTarget, FlgKpi = row.FlgKpi,
            ZeroInitial = row.ZeroInitial, PerfectExtreme = row.PerfectExtreme, WarnExtreme = row.WarnExtreme,
            MeasureConfirmStatus = row.MeasureConfirmStatus, Deleted = row.Deleted
        };
    }
}

public sealed class MeasureDetailsRow
{
    public long Id { get; set; }
    public string? StrategyObjectTitle { get; set; }
    public long? StrategyObjectId { get; set; }
    public string? MeasureName { get; set; }
    public string? Tashilgar { get; set; }
    public string? Modir { get; set; }
    public int MeasurePeriod { get; set; }
    public string? DesMeasureF { get; set; }
    public float? Weight { get; set; }
    public long MeasureCatalogId { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public decimal? OneYearLaterTarget { get; set; }
    public decimal? TwoYearLaterTarget { get; set; }
    public byte? DataType { get; set; }
    public int DesiredMethod { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool FlgKpi { get; set; }
    public bool ZeroInitial { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtreme { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public bool Deleted { get; set; }
}
