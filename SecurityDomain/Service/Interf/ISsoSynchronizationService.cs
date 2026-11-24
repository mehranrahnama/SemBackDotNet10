using SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>
/// معادل SSOSynchronizationService.java — اینترفیس همگام‌سازی کاربر با SSO.
/// org.springframework.security.core.userdetails.UserDetails → SemUserDetail.
/// </summary>
public interface ISsoSynchronizationService
{
    /// <summary>معادل synchronizeWithSSO(String username, String token).</summary>
    SemUserDetail? SynchronizeWithSSO(string username, string token);
}
