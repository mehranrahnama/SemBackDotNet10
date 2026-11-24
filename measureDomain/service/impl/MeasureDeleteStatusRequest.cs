using System.Globalization;
using System.Text.Json;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.measureDomain.service.impl;

// Request and validation rules only; the caller must resolve the measure and
// authorize access to its actual level before evaluating the deletion rules.
internal readonly record struct MeasureDeleteStatusRequest(long MeasureId, bool OnlyThisLevel)
{
    internal static MeasureDeleteStatusRequest Parse(JsonElement body)
    {
        if (body.ValueKind != JsonValueKind.Object)
            throw new BusinessException("بدنهٔ درخواست حذف شاخص باید یک شیء JSON باشد.");

        if (!body.TryGetProperty("id", out var id) || !TryReadId(id, out var measureId))
            throw new BusinessException("error.measure.idIsNotValid");

        var onlyThisLevel = false;
        if (body.TryGetProperty("onlyThisLevel", out var scope) && scope.ValueKind != JsonValueKind.Null)
        {
            if (scope.ValueKind is JsonValueKind.True or JsonValueKind.False)
                onlyThisLevel = scope.GetBoolean();
            else if (scope.ValueKind != JsonValueKind.String || !bool.TryParse(scope.GetString(), out onlyThisLevel))
                throw new BusinessException("مقدار onlyThisLevel باید درست یا نادرست باشد.");
        }
        return new MeasureDeleteStatusRequest(measureId, onlyThisLevel);
    }

    internal void Validate(bool alreadyDeleted, bool isAdmin, bool levelHasParent)
    {
        if (alreadyDeleted)
            throw new BusinessException("این شاخص در این کمیته در فرآیند حذف می باشد");
        if (!OnlyThisLevel) return;
        if (!isAdmin)
            throw new BusinessException("حذف شاخص از این سطح برنامه ریزی نیاز به دسترسی کاربر ارشد دارد.");
        if (levelHasParent)
            throw new BusinessException("حذف فقط همین سطح، در حال حاضر فقط برای بالاترین سطح کمیته امکان‌پذیر است.");
    }

    private static bool TryReadId(JsonElement id, out long value)
    {
        value = default;
        return id.ValueKind switch
        {
            JsonValueKind.Number => id.TryGetInt64(out value),
            JsonValueKind.String => long.TryParse(id.GetString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value),
            _ => false
        };
    }
}
