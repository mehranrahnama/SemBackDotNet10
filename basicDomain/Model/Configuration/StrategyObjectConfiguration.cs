#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Configuration;

public sealed class StrategyObjectConfiguration : IEntityTypeConfiguration<StrategyObject>
{
    public void Configure(EntityTypeBuilder<StrategyObject> builder)
    {
        builder.Property(x => x.Weight).HasConversion<decimal?>().HasPrecision(8, 4);
        // Java explicitly references tblplanperiod.planid rather than planperiodid.
        // PlanPeriod.PlanId must be unique for this relationship to be valid.
        builder.HasOne(x => x.PlanPeriod)
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .HasPrincipalKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
