// C# migration of enums/EntityNames.java

namespace Common.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityNames { MEASURE, MEASURE_ACT_PLAN, ACTION_ACT_PLAN, ACTION_PLAN_ACTIVITY }
public static class EntityNamesExtensions
{
 public static string GetDescription(this EntityNames value) => value switch {
 EntityNames.MEASURE => "Measure", EntityNames.MEASURE_ACT_PLAN => "MeasureActPlan",
 EntityNames.ACTION_ACT_PLAN => "ActionActPlan", EntityNames.ACTION_PLAN_ACTIVITY => "ActionPlanActivity",
 _ => throw new ArgumentOutOfRangeException(nameof(value)) };
}
