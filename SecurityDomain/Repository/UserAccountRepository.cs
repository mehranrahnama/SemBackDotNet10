using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IUserAccountRepository
{
    UserAccount? FindByUsername(string username);

    UserAccount? FindByProviderRegistrationIdAndProviderUsername(string registrationId, string providerUsername);

    UserAccount? FindSemAccountByUserId(long userId, string provider);

    List<UserAccount> FindNonSemAccountByUserId(long userId, string registrationId);

    List<UserAccount> FindByUserId(long userId);

    UserAccount? FindByIdAndProviderId(long id, long providerId);

    UserAccount? FindByAccountName(string accountName);

    UserAccount Save(UserAccount entity);

    void Delete(UserAccount entity);

    UserAccount? FindById(long id);
}

public class UserAccountRepository : SecurityRepositoryBase<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(AppDbContext context) : base(context) { }

    public UserAccount? FindByUsername(string username) =>
        Set.FirstOrDefault(a => a.AccountName == username);

    public UserAccount? FindByProviderRegistrationIdAndProviderUsername(string registrationId, string providerUsername) =>
        Set.FirstOrDefault(a => a.Provider.RegistrationId == registrationId && a.AccountName == providerUsername);

    public UserAccount? FindSemAccountByUserId(long userId, string provider) =>
        Set.FirstOrDefault(a => a.UserId == userId && a.Provider.RegistrationId == provider);

    public List<UserAccount> FindNonSemAccountByUserId(long userId, string registrationId) =>
        Set.Where(a => a.UserId == userId && a.Provider.RegistrationId != registrationId).ToList();

    public List<UserAccount> FindByUserId(long userId) => Set.Where(a => a.UserId == userId).ToList();

    public UserAccount? FindByIdAndProviderId(long id, long providerId) =>
        Set.FirstOrDefault(a => a.Id == id && a.ProviderId == providerId);

    public UserAccount? FindByAccountName(string accountName) =>
        Set.FirstOrDefault(a => a.AccountName == accountName);
}
