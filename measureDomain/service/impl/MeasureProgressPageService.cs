using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.measureDomain.repository;
using SemBackDotNet10.measureDomain.model.dto;
using global::Common.service.interf;
using global::Common.enums;

namespace SemBackDotNet10.service.impl;

// Internal application service; not registered as an endpoint. HOSCO correction
// synchronization must still be integrated before exposing this grid.
internal sealed class MeasureProgressPageService(ServiceRuntime runtime, StoredAttachmentService attachments,
    global::SemBackDotNet10.SecurityDomain.Interfaces.IRoleRepository roles,
    global::SemBackDotNet10.SecurityDomain.Tenancy.TenantContext tenant)
    : ServiceBase<PlaningLevel>(runtime)
{
    internal sealed record Input(long LevelId, long PlanId, long? Month, long? Status,
        long? ModirId, string? Filter, bool ModirView);
    internal async Task<ServicePage<MeasureProgressDto>> ReadAsync(Input input, int page, int size, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        var offset = checked(page * size);
        var userId = R.User.Id;
        await AccessAsync(input.LevelId, ct);
        if (input.ModirView && !await GetRepository().AnyAsync(level => level.Id == input.LevelId, ct))
            throw new global::SemBackDotNet10.Common.exception.BusinessException("error.PlaningLevel.idIsNotValid");
        var admin = R.User.IsAdmin;
        var roleNames = (input.ModirView && !admin) || input.Status == 40
            ? (await roles.GetForUserAsync(tenant.TenantId, userId, ct)).Select(role => role.Name).ToHashSet(StringComparer.Ordinal)
            : new HashSet<string>(StringComparer.Ordinal);
        var includeSublevels = input.ModirView && (admin || roleNames.Contains("Modiriat"));
        var sanaye = input.Status == 40 && roleNames.Contains("SanayeUser");
        var query = MeasureProgressMonthlyRows.Query((SemDbContext)R.Db, input.PlanId, input.LevelId, userId,
            includeSublevels, admin, sanaye, R.TenantName == "MSCHOLDING",
            input.Month, input.Status, input.ModirId, input.Filter);
        var (rows, total) = await MeasureProgressMonthlyRows.ReadPageAsync(query, offset, size, ct);
        var withFiles = rows.Count == 0 ? new HashSet<long>() : attachments.FindEntityIdsWithUploadedFile(
            EntityNames.MEASURE_ACT_PLAN, rows.Select(row => row.Id).ToList());
        var content = rows.Select(row => new MeasureProgressDto
        {
            Id = row.Id, MeasureName = row.MeasureName, MeasureUnitName = row.MeasureUnitName,
            MeasurePeriod = row.MeasurePeriod, DesiredMethod = row.DesiredMethod,
            MeasureType = row.MeasureType, MeasurePayeshType = row.MeasurePayeshType,
            PlanValue = row.PlanValue, MonthPlanValue = row.MonthPlanValue,
            ActualValue = row.ActualValue, DailyValue = row.DailyValue,
            Deviation = row.Deviation, Correction = row.Correction, MeasureActConfirmStatus = row.Status,
            LevelID = row.LevelId, Tashilgar = row.Tashilgar, TashilgarHamkar = row.TashilgarHamkar,
            Modir = row.Modir, ModirName = row.ModirName, Month = row.Month, PayeshgarName = row.PayeshgarName,
            Correctiondeadlinedate = row.CorrectionDeadlineDate, Correctionstartdateaction = row.CorrectionStartDateAction,
            Correctionexecuter = row.CatalogTashilgar, ExecuterName = row.ExecuterName,
            CorrectionActualValue = row.CorrectionActualValue, TaskUrl = row.TaskUrl, MeasurePCN = row.MeasurePCN,
            StrategyObjTitle = row.StrategyObjTitle, DesLevel = row.DesLevel,
            MeasureId = R.TenantName == "MSCHOLDING" ? row.MeasureId : null,
            IsRoot = row.IsRoot, HasUploadedFile = withFiles.Contains(row.Id)
        }).ToArray();
        return new(content, total, page, size);
    }
}
