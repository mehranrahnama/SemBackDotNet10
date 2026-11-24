using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Repositories;

/// <summary>
/// ریپازیتوری سبک کاربر برای سرویس‌های استراتژی و تیم
/// (معادل UserRepository جاوا؛ متدهای استفاده‌شده فقط).
/// </summary>
public interface IUserRepositoryForStrategy : IBaseRepository<User>
{
}

public class UserRepositoryForStrategy : BaseRepository<User>, IUserRepositoryForStrategy
{
    public UserRepositoryForStrategy(SemDbContext context) : base(context) { }
}
