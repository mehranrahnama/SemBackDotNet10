using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.service.impl;

internal sealed class MeasureProgressEmsCorrections(ICorrectionActionsByEMSService ems)
{
    // Returns the changed titles for persistence before publishing the page.
    internal Dictionary<long, string> Apply(IReadOnlyList<MeasureProgressDto> rows,
        Dictionary<string?, string?> headers, bool development)
    {
        var corrections = ems.retrieveAllCorrectionActionsByEMS(headers);
        var changed = new Dictionary<long, string>();
        if (development && (corrections is null || corrections.Count == 0)) return changed;
        if (corrections is null) throw new BusinessException("عدم دریافت اطلاعات اقدامات اصلاحی");
        var titles = new Dictionary<long, string?>();
        foreach (var correction in corrections)
            if (correction.IdentityCode is long id) titles.TryAdd(id, correction.Title);
        foreach (var row in rows)
        {
            if (row.Id is not long id || !titles.TryGetValue(id, out var title))
            {
                row.HasCorrectionEMS = false;
                continue;
            }
            if (title is null) throw new BusinessException("عنوان اقدام اصلاحی دریافت نشد");
            row.HasCorrectionEMS = true;
            if (row.Correction == title) continue;
            row.Correction = title;
            changed[id] = title;
        }
        return changed;
    }
}
