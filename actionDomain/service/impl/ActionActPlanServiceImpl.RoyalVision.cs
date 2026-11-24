using System;
using System.Globalization;
using System.Text.Json.Nodes;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public decimal? getCorrectionActualValueForHOSCO(long? actionActPlanId)
    {
        try
        {
            var plan = findByIdOrElseThrow(actionActPlanId);
            var result = _callActionRestApiService.getRoyalVisionCorrectionAct(plan.RoyalVisionActId);
            if (result is null || result.Count == 0) return null;
            var value = result["numProgressAction"];
            if (value is null) return 0m;
            var text = value is JsonValue scalar && scalar.TryGetValue<string>(out var str)
                ? str : value.ToJsonString();
            return decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            throw new BusinessException("خطا در دریافت اطلاعات");
        }
    }
}
