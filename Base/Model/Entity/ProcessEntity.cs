using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.Base.Model.Entity;

/// <summary>
/// معادل ProcessEntity در جاوا.
/// توجه: در جاوا این کلاس مستقیماً IIdentityCapable را پیاده می‌کند
/// (از BaseEntity ارث نمی‌برد)، اما همان ساختار Version و equals/hashCode را دارد؛
/// در سی‌شارپ برای پرهیز از تکرار، از BaseEntity ارث‌بری شده است.
/// </summary>
public abstract class ProcessEntity<TId> : BaseEntity<TId>, IProcessEntity
{
    private DateTime _createddate;
    private long _confirmstatus;
    private long _deleteFirstStatus;
    private long _replanStatusForHistory;
    private bool _replannedForHistory;
    private string? _outCome;

    /// <summary>معادل @CreatedBy با ستون createdby.</summary>
    [JsonIgnore]
    [Column("createdby")]
    public User? Createdby { get; set; }

    /// <summary>معادل @LastModifiedBy با ستون modifiedby.</summary>
    [JsonIgnore]
    [Column("modifiedby")]
    public User? Modifiedby { get; set; }

    /// <summary>معادل @CreationTimestamp (nullable = false, updatable = false).</summary>
    [JsonIgnore]
    [Column("createddate")]
    public DateTime Createddate
    {
        get => _createddate;
        set => _createddate = value;
    }

    /// <summary>معادل @UpdateTimestamp.</summary>
    [JsonIgnore]
    [Column("modifieddate")]
    public DateTime? Modifieddate { get; set; }

    [Column("confirmstatus")]
    public long Confirmstatus
    {
        get => _confirmstatus;
        set => _confirmstatus = value;
    }

    /// <summary>معادل @Transient — در پایگاه داده ذخیره نمی‌شود.</summary>
    [JsonIgnore]
    [NotMapped]
    public string? OutCome
    {
        get => _outCome;
        set => _outCome = value;
    }

    /// <summary>معادل @Transient (مقدار پیش‌فرض 0).</summary>
    [JsonIgnore]
    [NotMapped]
    public long DeleteFirstStatus
    {
        get => _deleteFirstStatus;
        set => _deleteFirstStatus = value;
    }

    /// <summary>معادل @Transient (مقدار پیش‌فرض 0).</summary>
    [JsonIgnore]
    [NotMapped]
    public long ReplanStatusForHistory
    {
        get => _replanStatusForHistory;
        set => _replanStatusForHistory = value;
    }

    /// <summary>معادل @Transient.</summary>
    [JsonIgnore]
    [NotMapped]
    public bool ReplannedForHistory
    {
        get => _replannedForHistory;
        set => _replannedForHistory = value;
    }

    long? IProcessEntity.ProcessId => Id is null ? null : Convert.ToInt64(Id);
}
