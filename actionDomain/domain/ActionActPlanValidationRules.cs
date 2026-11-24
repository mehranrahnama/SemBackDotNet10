#nullable enable
using System;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>Validation rules extracted from ActionActPlanServiceImpl.java.
/// Database reads and transaction ownership remain with the calling workflow.</summary>
public static class ActionActPlanValidationRules
{
    public static void validateActualValue(ActionActPlanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.ActualValue is null) return;
        if (dto.ActualValue.Value < 0m || dto.ActualValue.Value > 100m)
            throw new BusinessException("مقدار وارد شده باید در بازه 0-100 باشد.");
    }

    /// <param name="previousPlan">Result of the original lookup for catalog,
    /// current month minus one, and the same planId; not the previous calendar period.</param>
    public static void validatePreviousMonth(string? tenantName,
        ActionActPlan? previousPlan, ActionActPlanDto dto)
    {
        // The caller must also skip the repository lookup for IRISA.
        if (string.Equals(tenantName, "IRISA", StringComparison.Ordinal)) return;
        if (previousPlan is null) return;
        ArgumentNullException.ThrowIfNull(dto);
        if (previousPlan.ActualValue is null)
            throw new BusinessException("مقدار پیشرفت ماه قبل ثبت نشده است.");
        if (dto.ActualValue is not null && dto.ActualValue.Value < previousPlan.ActualValue.Value)
            throw new BusinessException("مقدار پیشرفت هر ماه باید بزرگتر یا مساوی ماه قبل باشد");
    }

    /// <param name="hasOldestDateLog">True when the date-log query returned a record,
    /// even if that record's EndDateOld is null.</param>
    public static void validateEndDateChange(string? tenantName,
        string? endDateOldFromCatalog, bool hasOldestDateLog,
        string? oldestLogEndDateOld, string? reason, string? newEndDate)
    {
        var previousDate = hasOldestDateLog ? oldestLogEndDateOld : endDateOldFromCatalog;
        var comparedResult = isBefore(previousDate, newEndDate);
        var missingReason = reason is null || JavaTrimIsEmpty(reason);
        var irisa = string.Equals(tenantName, "IRISA", StringComparison.Ordinal);
        var invalid = irisa
            ? newEndDate is not null && (comparedResult || missingReason)
            : newEndDate is null || comparedResult || missingReason;
        if (invalid)
            throw new BusinessException("تاریخ پایان جدید باید بزرگتر مساوی تاریخ قبلی باشد و دلیل تغییر تاریخ ذکر شده باشد.");
    }

    public static bool isBefore(string? prevDate, string? newDate)
        => prevDate is not null && newDate is not null
            && string.CompareOrdinal(newDate, prevDate) < 0;

    private static bool JavaTrimIsEmpty(string value)
    {
        // Java String.trim removes UTF-16 code units <= U+0020, not all
        // Unicode whitespace as .NET string.Trim/IsNullOrWhiteSpace do.
        foreach (var character in value)
            if (character > '\u0020') return false;
        return true;
    }
}
