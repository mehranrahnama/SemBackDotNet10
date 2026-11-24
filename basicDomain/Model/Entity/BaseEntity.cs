#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.Models.Entities;

/// <summary>Base persistence entity shared by every aggregate of the domain.</summary>
public class BaseEntity
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// Domain aggregates -----------------------------------------------------------

public class Delegation : BaseEntity
{
    public long DelegatorUserId { get; set; }
    public long DelegateUserId { get; set; }
    public DateTime? ExpiredDate { get; set; }
}

public class MainStrategy : BaseEntity
{
    public long LevelId { get; set; }
    public long PlanId { get; set; }
    public string? Code { get; set; }
}

public class MeasureUnit : BaseEntity
{
    public string? Symbol { get; set; }
    public bool IsForMenu { get; set; }
}

public class Organization : BaseEntity
{
    public long? ParentId { get; set; }
    public string? ManagerName { get; set; }
    public string? OrganizationCode { get; set; }
}

public class Perspective : BaseEntity
{
    public long LevelId { get; set; }
    public long PlanId { get; set; }
    public double Weight { get; set; }
}

public class PlaningLevel : BaseEntity
{
    public long? ParentLevelId { get; set; }
    public long PlanPeriodId { get; set; }
    public long UserId { get; set; }
    public bool IsSupremeCommittee { get; set; }
}

public class PlaningLevelOrganization : BaseEntity
{
    public long PlaningLevelId { get; set; }
    public long OrganizationId { get; set; }
}

public class PlaningLevelTeam : BaseEntity
{
    public long PlaningLevelId { get; set; }
    public long TeamId { get; set; }
}

public class PlaningLevelUnit : BaseEntity
{
    public long PlaningLevelId { get; set; }
    public long UnitId { get; set; }
}

public class PlaningLevelUser : BaseEntity
{
    public long PlaningLevelId { get; set; }
    public long UserId { get; set; }
}

public class PlanPeriod : BaseEntity
{
    public string? CalendarName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class Position : BaseEntity
{
    public long? OrganizationId { get; set; }
    public string? PositionCode { get; set; }
}

public class Setting : BaseEntity
{
    public long TenantId { get; set; }
    public string? SettingKey { get; set; }
    public string? SettingValue { get; set; }
}

public class StrategyObject : BaseEntity
{
    public long PerspectiveId { get; set; }
    public long LevelId { get; set; }
    public long PlanId { get; set; }
    public long? ParentId { get; set; }
    public string? ObjectFunctionality { get; set; }
}

[Table("tblstrategypillars", Schema = "dbo")]
public class StrategyPillar
{
    [Key, Column("strategypillarsid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Column("Levelid")]
    public long? LevelId { get; set; }
    [Column("planid")]
    public long? PlanId { get; set; }
    [ForeignKey(nameof(LevelId))]
    public global::SemBackDotNet10.basicDomain.Model.Entity.PlaningLevel? PlaningLevel { get; set; }
    [Required, Column("philosophy")]
    public string Philosophy { get; set; } = string.Empty;
    [Required, Column("mission")]
    public string Mission { get; set; } = string.Empty;
    [Required, Column("vision")]
    public string Vision { get; set; } = string.Empty;
    [Column("corporationvalues")]
    public string? CorporationValues { get; set; }
    [Column("abbreviationvalue")]
    public string? AbbreviationValue { get; set; }
    [Column("bigobjects")]
    public string? BigObjects { get; set; }
}

[Table("tblstrategypillarsmeasure", Schema = "dbo")]
public class StrategyPillarsMeasure
{
    [Key, Column("strategypillarsmeasureid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Column("strategypillarsid")]
    public long StrategyPillarsId { get; set; }
    [ForeignKey(nameof(StrategyPillarsId))]
    public StrategyPillar StrategyPillars { get; set; } = null!;
    [Required, Column("measurename")]
    public string MeasureName { get; set; } = string.Empty;
    [Column("measureplan")]
    public string? MeasurePlan { get; set; }
    [Column("measureunitid")]
    public long MeasureUnitId { get; set; }
    [ForeignKey(nameof(MeasureUnitId))]
    public global::SemBackDotNet10.basicDomain.Model.Entity.MeasureUnit MeasureUnit { get; set; } = null!;
    [Column("year")]
    public long Year { get; set; }
    [Column("isdeleted")]
    public bool Deleted { get; set; }
}

public enum MeasureTrend
{
    ASCENDING, STRICTLY_ASCENDING, DESCENDING, STRICTLY_DESCENDING
}

[Table("tblsystemconfiguration", Schema = "dbo")]
public class SystemConfiguration : BaseEntity
{
    [NotMapped]
    public string? ConfigKey { get; set; }
    [NotMapped]
    public string? ConfigValue { get; set; }

    [Column("tenantid")]
    public long? TenantId { get; set; }
    public global::SemBackDotNet10.basicDomain.Model.Entity.Tenant? Tenant { get; set; }

    [Column("multitargetactionplan")]
    public bool MultiTargetActionPlan { get; set; }

    [Column("measuretrend")]
    public MeasureTrend? MeasureTrend { get; set; }
}

public class User : BaseEntity
{
    public string? AccountName { get; set; }
    public string? FullName { get; set; }
    public string? Mobile { get; set; }
    public bool IsLocked { get; set; }
}
