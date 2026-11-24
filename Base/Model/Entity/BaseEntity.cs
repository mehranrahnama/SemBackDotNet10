using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.Base.Model.Entity;

public abstract class BaseEntity<ID> : IIdentityCapable<ID>
{
    [JsonIgnore]
    [Column("VERSION")]
    [ConcurrencyCheck]   // EF Core equivalent of @Version
    public long Version { get; set; }

    // *******************************
    // Override Equals
    // *******************************
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<ID> other)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return EqualityComparer<ID>.Default.Equals(Id, other.Id);
    }

    // *******************************
    // Override GetHashCode
    // *******************************
    public override int GetHashCode()
    {
        return EqualityComparer<ID>.Default.GetHashCode(Id!);
    }

    // *******************************
    // Identity Interface Contract
    // Implemented in child classes
    // *******************************
    public virtual ID Id { get; set; } = default!;
}
