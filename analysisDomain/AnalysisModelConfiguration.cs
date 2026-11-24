using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.analysisDomain.util;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain;

public static class AnalysisModelConfiguration
{
    private const string EntityNamespacePrefix = "SemBackDotNet10.analysisDomain.model.entity.";

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlanPeriod>().HasAlternateKey(x => x.PlanId);

        var entityTypes = typeof(AnalysisModelConfiguration).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null
                        && t.Namespace.StartsWith(EntityNamespacePrefix, StringComparison.Ordinal)
                        && t.Namespace != EntityNamespacePrefix + "enums"
                        && t.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), false).Length > 0);

        foreach (var type in entityTypes)
        {
            var builder = modelBuilder.Entity(type);
            builder.Ignore("CreatedBy");
            builder.Ignore("ModifiedBy");

            if (type.GetProperty("PlanPeriod") != null && type.GetProperty("PlanId") != null)
            {
                builder.HasOne(typeof(PlanPeriod), "PlanPeriod")
                    .WithMany()
                    .HasForeignKey("PlanId")
                    .HasPrincipalKey(nameof(PlanPeriod.PlanId))
                    .OnDelete(DeleteBehavior.NoAction);
            }
        }

        modelBuilder.Entity<model.entity.pestel.Pestel>().Property(x => x.Impact).HasConversion<string>();
        modelBuilder.Entity<model.entity.pestel.Pestel>().Property(x => x.Uncertainty).HasConversion<string>();
        modelBuilder.Entity<model.entity.stakeHolder.StakeHolder>().Property(x => x.BehaviorStrategy).HasConversion<string>();
        modelBuilder.Entity<model.entity.stakeHolder.StakeholderNeedsPriority>().Property(x => x.AnsweringType).HasConversion<string>();
        modelBuilder.Entity<model.entity.porter.ProductThreat>().Property(x => x.Threat).HasConversion(new ThreatConverter());
        modelBuilder.Entity<model.entity.porter.PorterResult>().Property(x => x.Result).HasConversion(new ThreatConverter());
    }
}
