using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>
/// معادل UserAccountService.java — اینترفیس سرویس حساب کاربری.
/// UserDTO جاوا از actionDomain.controller.dto است؛ تا تبدیل آن، از UserAccountDto استفاده شده است.
/// </summary>
public interface IUserAccountService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IUserAccountRepository GetRepository();

    /// <summary>معادل findByIdOrElseThrow(Long).</summary>
    Task<UserAccount> FindByIdOrElseThrowAsync(long id, CancellationToken ct = default);

    /// <summary>معادل findByUsername(String).</summary>
    Task<UserAccount?> FindByUsernameAsync(string userName, CancellationToken ct = default);

    /// <summary>معادل findByUsernameForSwitchUser(String).</summary>
    Task<UserAccount?> FindByUsernameForSwitchUserAsync(string userName, CancellationToken ct = default);

    /// <summary>معادل findByProviderAndProviderUsername(String, String).</summary>
    Task<UserAccount?> FindByProviderAndProviderUsernameAsync(string provider, string userName, CancellationToken ct = default);

    /// <summary>معادل findSemAccountByUser(Long).</summary>
    Task<UserAccount?> FindSemAccountByUserAsync(long userId, CancellationToken ct = default);

    /// <summary>معادل findNonSemAccountByUser(Long).</summary>
    Task<List<UserAccount>> FindNonSemAccountByUserAsync(long userId, CancellationToken ct = default);

    /// <summary>معادل mapToDtoByPasswordAndUserAndRoles(UserAccount).</summary>
    UserAccountDto MapToDtoByPasswordAndUserAndRoles(UserAccount account);

    /// <summary>معادل createAccountUser(String, UserAccount, OauthProvider).</summary>
    UserAccountDto CreateAccountUser(string username, UserAccount account, OauthProvider oauthProvider);

    /// <summary>معادل createAccount(UserDTO).</summary>
    UserAccountDto CreateAccount(UserAccountDto userAccountDto);

    /// <summary>معادل editAccount(UserAccountDto).</summary>
    UserAccountDto EditAccount(UserAccountDto userAccountDto);

    /// <summary>معادل changePassword(String).</summary>
    void ChangePassword(string body);

    /// <summary>معادل deleteByUser(Long).</summary>
    Task<bool> DeleteByUserAsync(long userId, CancellationToken ct = default);

    /// <summary>معادل resetPassword(Long, String).</summary>
    void ResetPassword(long userId, string body);

    /// <summary>معادل createLDAPAccount(String).</summary>
    UserAccountDto CreateLdapAccount(string userName);

    /// <summary>معادل findByUserId(Long).</summary>
    Task<List<UserAccount>> FindByUserIdAsync(long userId, CancellationToken ct = default);
}
