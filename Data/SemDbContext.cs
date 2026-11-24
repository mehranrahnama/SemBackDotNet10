using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.Data;

/// <summary>
/// EF Core context for the BasicDomain entities.
/// SecurityDomain.Persistence.AppDbContext remains the security context.
/// </summary>
public sealed class SemDbContext(DbContextOptions<SemDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Delegation> Delegations => Set<Delegation>();
    public DbSet<MeasureUnit> MeasureUnits => Set<MeasureUnit>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<PlaningLevel> PlaningLevels => Set<PlaningLevel>();
    public DbSet<PlaningLevelUser> PlaningLevelUsers => Set<PlaningLevelUser>();
    public DbSet<PlaningLevelTeam> PlaningLevelTeams => Set<PlaningLevelTeam>();
    public DbSet<PlaningLevelUnit> PlaningLevelUnits => Set<PlaningLevelUnit>();
    public DbSet<PlaningLevelOrganization> PlaningLevelOrganizations => Set<PlaningLevelOrganization>();
    public DbSet<PlanPeriod> PlanPeriods => Set<PlanPeriod>();
    public DbSet<Perspective> Perspectives => Set<Perspective>();
    public DbSet<MainStrategy> MainStrategies => Set<MainStrategy>();
    public DbSet<StrategyObject> StrategyObjects => Set<StrategyObject>();
    public DbSet<MeasureCatalogItem> MeasureCatalogItems => Set<MeasureCatalogItem>();
    public DbSet<ActionPlan> ActionPlans => Set<ActionPlan>();
    public DbSet<SemBackDotNet10.actionDomain.model.entity.ActionPlan> DomainActionPlans => Set<SemBackDotNet10.actionDomain.model.entity.ActionPlan>();
    public DbSet<SemBackDotNet10.actionDomain.model.entity.ActionActPlan> ActionActPlans => Set<SemBackDotNet10.actionDomain.model.entity.ActionActPlan>();
    public DbSet<SemBackDotNet10.actionDomain.model.entity.ActionPlanCatalogRelation> ActionPlanCatalogRelations => Set<SemBackDotNet10.actionDomain.model.entity.ActionPlanCatalogRelation>();
    public DbSet<SemBackDotNet10.actionDomain.model.entity.ActionActPlanLog> ActionActPlanLogs => Set<SemBackDotNet10.actionDomain.model.entity.ActionActPlanLog>();
    public DbSet<global::Common.model.entity.History> Histories => Set<global::Common.model.entity.History>();
    public DbSet<global::Common.model.entity.Comment> Comments => Set<global::Common.model.entity.Comment>();
    public DbSet<global::Common.model.entity.ReportExecutedData> ReportExecutedData => Set<global::Common.model.entity.ReportExecutedData>();
    public DbSet<SemBackDotNet10.dashboardDomain.model.entity.Dashboard> Dashboards => Set<SemBackDotNet10.dashboardDomain.model.entity.Dashboard>();
    public DbSet<SemBackDotNet10.dashboardDomain.model.entity.DashboardReportAllLevelScore> DashboardReportAllLevelScores => Set<SemBackDotNet10.dashboardDomain.model.entity.DashboardReportAllLevelScore>();
    public DbSet<SemBackDotNet10.dashboardDomain.model.entity.ReportDashboardPayeshPCN> ReportDashboardPayeshPCNs => Set<SemBackDotNet10.dashboardDomain.model.entity.ReportDashboardPayeshPCN>();
    public DbSet<SemBackDotNet10.dashboardDomain.model.entity.StrategyMapEvalByMeasureHoscoDto> StrategyMapEvalByMeasureHoscos => Set<SemBackDotNet10.dashboardDomain.model.entity.StrategyMapEvalByMeasureHoscoDto>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.pestel.Pestel> Pestels => Set<SemBackDotNet10.analysisDomain.model.entity.pestel.Pestel>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelGroup> PestelGroups => Set<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelGroup>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelScore> PestelScores => Set<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelScore>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.AltProduct> AltProducts => Set<SemBackDotNet10.analysisDomain.model.entity.porter.AltProduct>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.Market> Markets => Set<SemBackDotNet10.analysisDomain.model.entity.porter.Market>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.Porter> Porters => Set<SemBackDotNet10.analysisDomain.model.entity.porter.Porter>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.PorterGroup> PorterGroups => Set<SemBackDotNet10.analysisDomain.model.entity.porter.PorterGroup>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.PorterResult> PorterResults => Set<SemBackDotNet10.analysisDomain.model.entity.porter.PorterResult>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.PorterScore> PorterScores => Set<SemBackDotNet10.analysisDomain.model.entity.porter.PorterScore>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.Product> Products => Set<SemBackDotNet10.analysisDomain.model.entity.porter.Product>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.ProductGroup> ProductGroups => Set<SemBackDotNet10.analysisDomain.model.entity.porter.ProductGroup>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.porter.ProductThreat> ProductThreats => Set<SemBackDotNet10.analysisDomain.model.entity.porter.ProductThreat>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.resource.Resource> Resources => Set<SemBackDotNet10.analysisDomain.model.entity.resource.Resource>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.resource.ResourceCompetencyRule> ResourceCompetencyRules => Set<SemBackDotNet10.analysisDomain.model.entity.resource.ResourceCompetencyRule>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeHolder> StakeHolders => Set<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeHolder>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeHolderGroup> StakeHolderGroups => Set<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeHolderGroup>();
    public DbSet<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeholderNeedsPriority> StakeholderNeedsPriorities => Set<SemBackDotNet10.analysisDomain.model.entity.stakeHolder.StakeholderNeedsPriority>();
    public DbSet<Measure> Measures => Set<Measure>();
    public DbSet<SemBackDotNet10.webserviceDomain.domain.entity.ApiKeys> ApiKeys => Set<SemBackDotNet10.webserviceDomain.domain.entity.ApiKeys>();
    public DbSet<SemBackDotNet10.riskDomain.model.entity.Risk> Risks => Set<SemBackDotNet10.riskDomain.model.entity.Risk>();
    public DbSet<SemBackDotNet10.riskDomain.model.entity.RiskMeasure> RiskMeasures => Set<SemBackDotNet10.riskDomain.model.entity.RiskMeasure>();
    public DbSet<SemBackDotNet10.riskDomain.model.entity.RiskAction> RiskActions => Set<SemBackDotNet10.riskDomain.model.entity.RiskAction>();
    public DbSet<EvaluationTeamHeaderLevel> EvaluationTeamHeaderLevels => Set<EvaluationTeamHeaderLevel>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<DomainEventOutbox> DomainEventOutbox => Set<DomainEventOutbox>();

    public Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<global::Common.model.entity.Des>()
            .HasOne(d => d.DesGrp).WithMany().HasForeignKey("desgrpid");
        modelBuilder.Entity<SemBackDotNet10.dashboardDomain.model.entity.Dashboard>().Property(x => x.CreatedById).HasColumnName("createdby");
        modelBuilder.Entity<SemBackDotNet10.dashboardDomain.model.entity.Dashboard>().Property(x => x.ModifiedById).HasColumnName("modifiedby");

        // Java schema stores tenant keys as SQL int; the domain model exposes long.
        modelBuilder.Entity<Tenant>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<Tenant>().Property(x => x.TenantParentId).HasConversion<int?>();
        modelBuilder.Entity<MeasureUnit>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<MainStrategy>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<MainStrategy>().Property(x => x.AccountableId).HasConversion<int?>();
        modelBuilder.Entity<MainStrategy>().Property(x => x.PerspectiveId).HasConversion<int?>();
        modelBuilder.Entity<MainStrategy>().Property(x => x.PlaningLevelId).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<Perspective>().Property(x => x.PlaningLevelId).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.ParentId).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.PlanPeriodId).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.OriginId).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.Orders).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.Rank).HasConversion<int?>();
        modelBuilder.Entity<Perspective>().Property(x => x.Weight).HasConversion<byte?>();
        modelBuilder.Entity<StrategyObject>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<StrategyObject>().Property(x => x.PerspectiveId).HasConversion<int?>();
        modelBuilder.Entity<StrategyObject>().Property(x => x.PlaningLevelId).HasConversion<int?>();
        modelBuilder.Entity<StrategyObject>().Property(x => x.PlanId).HasConversion<int?>();
        modelBuilder.Entity<global::Common.model.entity.CartableStatus>().Property(x => x.Id).HasConversion<int?>();
        modelBuilder.Entity<global::Common.model.entity.CartableStatus>().Property(x => x.OldStatus).HasConversion<int?>();
        modelBuilder.Entity<global::Common.model.entity.CartableStatus>().Property(x => x.NewStatus).HasConversion<int?>();
        modelBuilder.Entity<global::Common.model.entity.CartableStatus>().Property(x => x.FromRoleId).HasConversion<int?>();
        modelBuilder.Entity<global::Common.model.entity.CartableStatus>().Property(x => x.ToRoleId).HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillar>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillar>().Property(x => x.PlanId).HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillar>().Property(x => x.LevelId).HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillarsMeasure>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillarsMeasure>().Property(x => x.StrategyPillarsId).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillarsMeasure>().Property(x => x.MeasureUnitId).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.Models.Entities.StrategyPillarsMeasure>().Property(x => x.Year).HasConversion<int>();
        modelBuilder.Entity<Position>().Ignore("CreatedBy");
        modelBuilder.Entity<Position>().Ignore("ModifiedBy");
        modelBuilder.Entity<Position>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<Position>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<Position>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<PlaningLevelUser>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<PlaningLevelUser>().Property(x => x.UserId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevelUser>().Property(x => x.LevelId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevelUser>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<PlaningLevelUser>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.ParentId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.ResponsibleId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.FacilitatorId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.ModirId).HasConversion<int?>();
        modelBuilder.Entity<PlaningLevel>().Property(x => x.TashilgarId).HasConversion<int?>();
        modelBuilder.Entity<PlanPeriod>().Property(x => x.PlanId).HasConversion<int>();
        modelBuilder.Entity<PlanPeriod>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<PlanPeriod>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("tblUser", "dbo");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("UserId").HasConversion<int>();
            entity.Property(x => x.TenantId).HasColumnName("TenantId").HasConversion<int?>();
            entity.Ignore(x => x.Type);
        });
        modelBuilder.Entity<SemBackDotNet10.service.impl.Setting>(entity =>
        {
            entity.ToTable("tblSetting", "dbo");
            entity.HasKey(x => x.SettingId);
            entity.Property(x => x.SettingId).HasColumnName("settingId").HasConversion<int>().ValueGeneratedOnAdd();
            entity.Property(x => x.TenantId).HasColumnName("tenantId").HasConversion<int>();
            entity.Property(x => x.SettingKey).HasColumnName("settingKey");
            entity.Property(x => x.SettingValue).HasColumnName("settingValue");
        });

        var delegation = modelBuilder.Entity<Delegation>();
        delegation.Property(x => x.Id).HasConversion<int>();
        delegation.Property(x => x.FromUserId).HasConversion<int>();
        delegation.Property(x => x.ToUserId).HasConversion<int>();
        delegation.Property(x => x.DelegationStatus).HasConversion<int>();
        delegation.HasOne(x => x.FromUser).WithMany().HasForeignKey(x => x.FromUserId).OnDelete(DeleteBehavior.NoAction);
        delegation.HasOne(x => x.ToUser).WithMany().HasForeignKey(x => x.ToUserId).OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SemBackDotNet10.Models.Entities.SystemConfiguration>()
            .Property(x => x.MeasureTrend).HasConversion<string>();

        // These navigations point to SecurityDomain entities and belong to
        // AppDbContext, not this BasicDomain context.
        modelBuilder.Entity<User>().Ignore(x => x.Accounts);
        foreach (var entityType in new[]
        {
            typeof(User),
            typeof(PlaningLevel),
            typeof(PlaningLevelUser),
            typeof(PlaningLevelTeam),
            typeof(PlaningLevelOrganization),
            typeof(PlanPeriod),
            typeof(Tenant)
        })
        {
            modelBuilder.Entity(entityType).Ignore("CreatedBy");
            modelBuilder.Entity(entityType).Ignore("ModifiedBy");
        }

        // The entity attributes define the legacy table and column names.
        // Explicit relationships are added here only where the model exposes them.
        modelBuilder.Entity<PlaningLevelUser>()
            .HasIndex(x => new { x.UserId, x.LevelId })
            .IsUnique();

        modelBuilder.Entity<PlanPeriod>()
            .HasIndex(x => x.PlanId)
            .IsUnique();

        modelBuilder.Entity<Perspective>()
            .HasOne(x => x.PlanPeriod).WithMany()
            .HasPrincipalKey(x => x.PlanId)
            .HasForeignKey(x => x.PlanPeriodId);
        modelBuilder.ApplyConfiguration(new SemBackDotNet10.basicDomain.Model.Configuration.StrategyObjectConfiguration());

        modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionActPlan>()
            .HasOne(x => x.ActionPlanCatalog).WithMany(x => x.ActionActPlans)
            .HasForeignKey("actionplancatalogid");
        modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlan>()
            .HasOne(x => x.ActionPlanCatalog).WithMany(x => x.ActionPlans)
            .HasForeignKey("actionplancatalogid");
        modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionActPlan>()
            .HasOne(x => x.PlanPeriod).WithMany().HasPrincipalKey(x => x.PlanId)
            .HasForeignKey("planid");
        modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlan>()
            .HasOne(x => x.PlanPeriod).WithMany().HasPrincipalKey(x => x.PlanId)
            .HasForeignKey("planid");
        var actionPlan = modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlan>();
        actionPlan.Property(x => x.Id).HasConversion<int>();
        actionPlan.Property(x => x.Priority).HasConversion<byte>();
        actionPlan.Property(x => x.Type).HasConversion<byte>();
        actionPlan.Property(x => x.Confirmstatus).HasConversion<int?>();
        actionPlan.Property(x => x.Weight).HasConversion<decimal?>();
        actionPlan.HasOne(x => x.StrategyObject).WithMany().HasForeignKey("strategyobjectid").IsRequired(false);
        actionPlan.HasOne(x => x.PlaningLevel).WithMany().HasForeignKey("levelid").IsRequired(false);
        actionPlan.HasOne(x => x.Modir).WithMany().HasForeignKey("modir").IsRequired(false);
        actionPlan.HasOne(x => x.Payeshgar).WithMany().HasForeignKey("payeshgar").IsRequired(false);
        actionPlan.HasOne(x => x.Createdby).WithMany().HasForeignKey("createdby").IsRequired(false);
        actionPlan.HasOne(x => x.Modifiedby).WithMany().HasForeignKey("modifiedby").IsRequired(false);
        var actionCatalog = modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlanCatalog>();
        actionCatalog.Property(x => x.Id).HasConversion<int>();
        actionCatalog.HasOne(x => x.PlaningLevel).WithMany().HasForeignKey("levelid").IsRequired(false);
        actionCatalog.HasOne(x => x.Modir).WithMany().HasForeignKey("modir").IsRequired(false);
        actionCatalog.HasOne(x => x.Payeshgar).WithMany().HasForeignKey("payeshgar").IsRequired(false);
        actionCatalog.HasOne(x => x.Createdby).WithMany().HasForeignKey("createdby").IsRequired(false);
        actionCatalog.HasOne(x => x.Modifiedby).WithMany().HasForeignKey("modifiedby").IsRequired(false);
        var actionActivity = modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlanActivity>();
        actionActivity.Property(x => x.Id).HasConversion<int>();
        actionActivity.Property(x => x.Weight).HasConversion<decimal?>();
        actionActivity.HasOne(x => x.ActionPlanCatalog).WithMany().HasForeignKey("actionplancatalogid").IsRequired(false);
        actionActivity.HasOne(x => x.Accountable).WithMany().HasForeignKey("accountable").IsRequired(false);
        var actionBeneficiary = modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlanBeneficiary>();
        actionBeneficiary.Property(x => x.Id).HasConversion<int>();
        actionBeneficiary.HasOne(x => x.ActionPlan).WithMany().HasForeignKey("actionplanid").IsRequired(false);
        actionBeneficiary.HasOne(x => x.PlaningLevelOrganization).WithMany().HasForeignKey("planinglevelorgid").IsRequired(false);
        var actionCoworker = modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionPlanCoworker>();
        actionCoworker.Property(x => x.Id).HasConversion<int>();
        actionCoworker.HasOne(x => x.ActionPlan).WithMany().HasForeignKey("actionplanid").IsRequired(false);
        actionCoworker.HasOne(x => x.PlaningLevelOrganization).WithMany().HasForeignKey("planinglevelorgid").IsRequired(false);
        modelBuilder.Entity<SemBackDotNet10.actionDomain.model.entity.ActionActPlanLog>()
            .HasOne(x => x.PlanPeriod).WithMany().HasPrincipalKey(x => x.PlanId)
            .HasForeignKey("planid");

        var evaluationHeader = modelBuilder.Entity<SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeamHeader>();
        evaluationHeader.ToTable("tblevaluationteamheader", "eval");
        evaluationHeader.HasKey(x => x.Id);
        evaluationHeader.Property(x => x.Id).HasColumnName("evaluationteamheaderid").ValueGeneratedOnAdd();
        evaluationHeader.Property(x => x.FromMonth).HasColumnName("frommonth");
        evaluationHeader.Property(x => x.ToMonth).HasColumnName("tomonth");
        evaluationHeader.Property(x => x.CreatedById).HasColumnName("createdby");
        evaluationHeader.Property(x => x.ModifiedById).HasColumnName("modifiedby");
        evaluationHeader.Ignore(x => x.CreatedBy);
        evaluationHeader.Ignore(x => x.ModifiedBy);
        evaluationHeader.HasOne(x => x.PlanPeriod).WithMany().HasPrincipalKey(x => x.PlanId).HasForeignKey("planid").IsRequired(false);
        evaluationHeader.HasOne(x => x.MasterRater).WithMany().HasForeignKey("masterrater").IsRequired(false);

        var evaluationLevel = modelBuilder.Entity<SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeamHeaderLevel>();
        evaluationLevel.ToTable("tblevaluationteamheaderlevel", "eval");
        evaluationLevel.HasKey(x => x.Id);
        evaluationLevel.Property(x => x.Id).HasColumnName("evaluationteamheaderlevelid").ValueGeneratedOnAdd();
        evaluationLevel.HasOne(x => x.EvaluationTeamHeader).WithMany(x => x.EvaluationTeamHeaderLevelList).HasForeignKey("evaluationteamheaderid").IsRequired(false);
        evaluationLevel.HasOne(x => x.PlaningLevel).WithMany().HasForeignKey("levelid").IsRequired(false);

        var evaluationTeam = modelBuilder.Entity<SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeam>();
        evaluationTeam.ToTable("tblevaluationteam", "eval");
        evaluationTeam.HasKey(x => x.Id);
        evaluationTeam.Property(x => x.Id).HasColumnName("evaluationteamid").ValueGeneratedOnAdd();
        evaluationTeam.Ignore(x => x.EvaluationTeamHeader);
        evaluationTeam.Ignore(x => x.PlaningLevel);
        evaluationTeam.HasOne(x => x.EvaluationTeamHeaderLevel).WithMany().HasForeignKey("evaluationteamheaderlevelid").IsRequired(false);
        evaluationTeam.Property<long?>("RaterId").HasColumnName("Rater");
        evaluationTeam.HasOne(x => x.Rater).WithMany().HasForeignKey("RaterId").IsRequired(false);

        var pestelGroup = modelBuilder.Entity<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelGroup>();
        pestelGroup.Property(x => x.CreatedById).HasColumnName("createdby").HasConversion<int?>();
        pestelGroup.Property(x => x.ModifiedById).HasColumnName("modifiedby").HasConversion<int?>();

        modelBuilder.Entity<SemBackDotNet10.analysisDomain.model.entity.pestel.PestelScore>()
            .HasOne(x => x.PlanPeriod).WithMany().HasPrincipalKey(x => x.PlanId)
            .HasForeignKey(x => x.PlanId);

        modelBuilder.Entity<StrategyObject>()
            .HasQueryFilter(x => x.Deleted == false || x.Deleted == null);

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Use the actual security tables in the same context and transaction as
        // planning-level changes; the legacy support entities are separate types.
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.Role>().Ignore("CreatedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.Role>().Ignore("ModifiedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.Role>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.Role>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.Role>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Ignore("CreatedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Ignore("ModifiedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Property(x => x.Id).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Property(x => x.UserId).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Property(x => x.RoleId).HasConversion<int>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserAccount>().Ignore("CreatedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserAccount>().Ignore("ModifiedBy");
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserAccount>()
            .HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>()
            .HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>()
            .HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.NoAction);

        SemBackDotNet10.analysisDomain.AnalysisModelConfiguration.Apply(modelBuilder);

        foreach (var riskEntity in new[]
        {
            typeof(SemBackDotNet10.riskDomain.model.entity.Risk),
            typeof(SemBackDotNet10.riskDomain.model.entity.RiskMeasure),
            typeof(SemBackDotNet10.riskDomain.model.entity.RiskAction)
        })
        {
            modelBuilder.Entity(riskEntity).Ignore("CreatedBy");
            modelBuilder.Entity(riskEntity).Ignore("ModifiedBy");
        }

        modelBuilder.Entity<SemBackDotNet10.webserviceDomain.domain.entity.ApiKeys>().Ignore("CreatedBy");
        modelBuilder.Entity<SemBackDotNet10.webserviceDomain.domain.entity.ApiKeys>().Ignore("ModifiedBy");
    }
}
