namespace SemBackDotNet10.Base.Model.Entity;

/// <summary>
/// معادل IdentityCapable در جاوا:
/// هر موجودیتی که شناسه (کلید اصلی) دارد، این قرارداد را پیاده‌سازی می‌کند.
/// </summary>
public interface IIdentityCapable<TId>
{
    TId Id { get; set; }
}
