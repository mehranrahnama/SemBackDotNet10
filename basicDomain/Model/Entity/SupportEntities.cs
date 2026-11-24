using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

/// <summary>کاتالوگ شاخص (حداقلی – برای getUserLevelIds).</summary>
public class MeasureCatalogItem
{
    public long Id { get; set; }
    public long? LevelId { get; set; }
    public PlaningLevel? Level { get; set; }
    public string? Title { get; set; }
}

/// <summary>برنامه اقدام (حداقلی – برای hardDeleteActionPlan).</summary>
public class ActionPlan
{
    public long Id { get; set; }
    public long? StrategyObjectId { get; set; }
    public StrategyObject? StrategyObject { get; set; }
    public string? DesActionPlanF { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>شاخص (حداقلی – برای hardDeleteMeasure).</summary>
public class Measure
{
    public long Id { get; set; }
    public long? StrategyObjectId { get; set; }
    public StrategyObject? StrategyObject { get; set; }
    public string? DesMeasureF { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>سرپ تیم ارزیابی به‌ازای کمیته (برای getUserLevelIds).</summary>
public class EvaluationTeamHeaderLevel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long LevelId { get; set; }
    public PlaningLevel? Level { get; set; }
}

/// <summary>نقش کاربر (برای userIsAdmin).</summary>
public class UserRoleEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long RoleId { get; set; }
    public RoleEntity? Role { get; set; }
}

/// <summary>نقش.</summary>
public class RoleEntity
{
    public long Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

/// <summary>Outbox ساده برای انتشار رویدادهای دامنه بعد از Commit تراکنش.</summary>
public class DomainEventOutbox
{
    public long Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public bool Processed { get; set; }
}
