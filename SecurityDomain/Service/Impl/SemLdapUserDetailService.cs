using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using IRoleRepository = global::SemBackDotNet10.SecurityDomain.Interfaces.IRoleRepository;
using ITenantService = global::SemBackDotNet10.basicDomain.service.ITenantService;
using TenantDto = global::SemBackDotNet10.basicDomain.Model.Dto.TenantDto;
using JwtTokenProvider = global::SemBackDotNet10.SecurityDomain.tokenprovider.JwtTokenProvider;
using SemLogger = global::Common.logging.SemLogger;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.SemUserDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل SemLdapUserDetailService.java (implements UserDetailsService).
/// UserDetailsService جاوا → متد LoadUserByUsername(string) که SemUserDetail (معادل UserDetails) برمی‌گرداند.
/// @Bean RequestContextListener در Spring → IHttpContextAccessor ثبت‌شده در DI (AddHttpContextAccessor)؛
///   متد Bean حذف شد و در کامنت راهنما آمده است.
/// وابستگی‌های اعلام‌شده (هنوز تبدیل نشده): JwtTokenProvider، IOauthProviderService، IUserAccountService،
///   ITenantService، IRoleRepository، ApplicationConfigService، IUserService، SemUserDetail، UserAccountDto.
/// </summary>
public class SemLdapUserDetailService
{
    private const string SelfHosted = "SELF_HOST";

    private readonly JwtTokenProvider _jwtTokenProvider;
    private readonly IOauthProviderService _oauthProviderService;
    private readonly IUserAccountService _accountService;
    private readonly ITenantService _tenantService;
    private readonly IRoleRepository _roleRepository;
    private readonly ApplicationConfigService _applicationConfigService;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SemLogger _semLogger;

    public SemLdapUserDetailService(JwtTokenProvider jwtTokenProvider, IOauthProviderService oauthProviderService,
        IUserAccountService accountService, ITenantService tenantService,
        IRoleRepository roleRepository, ApplicationConfigService applicationConfigService, IUserService userService,
        IHttpContextAccessor httpContextAccessor, SemLogger semLogger)
    {
        _jwtTokenProvider = jwtTokenProvider;
        _oauthProviderService = oauthProviderService;
        _accountService = accountService;
        _tenantService = tenantService;
        _roleRepository = roleRepository;
        _applicationConfigService = applicationConfigService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _semLogger = semLogger;
    }

    /// <summary>
    /// معادل loadUserByUsername(String) — از RequestContextHolder جاوا به‌جای آن
    /// IHttpContextAccessor.HttpContext استفاده می‌شود؛ اگر HttpContext null باشد، در جاوا هم استثنا می‌رفت.
    /// </summary>
    public SemUserDetail? LoadUserByUsername(string username)
    {
        _semLogger.Info("Enter the method");
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext — register AddHttpContextAccessor()");
        var curOrgan = _jwtTokenProvider.GetCurrentOrganizationAsync(context.Request, context.RequestAborted)
            .GetAwaiter().GetResult();

        if (!string.IsNullOrWhiteSpace(curOrgan))
        {
            var oauthProvider = _oauthProviderService.FindByTenantIdAndTypeAsync(
                long.Parse(curOrgan), OauthProvider.ProviderType.LDAP.ToString(), context.RequestAborted)
                .GetAwaiter().GetResult();
            if (oauthProvider != null)
            {
                try
                {
                    var result = LoadUserByUsername(username, oauthProvider, curOrgan);
                    _semLogger.Info("Exit method");
                    return result;
                }
                catch (Exception e)
                {
                    _semLogger.Error("throw exception: " + e.Message);
                    throw new BusinessException(e.Message);
                }
            }
        }
        _semLogger.Info("Exit method");
        return null;
    }

    public SemUserDetail LoadUserByUsername(string username, OauthProvider oauthProvider, string curOrgan)
    {
        _semLogger.Info("Enter the method");
        var accountOptional = _accountService.FindByProviderAndProviderUsernameAsync(
            oauthProvider.RegistrationId,
            username + "_" + oauthProvider.RegistrationId,
            _httpContextAccessor.HttpContext?.RequestAborted ?? default)
            .GetAwaiter().GetResult();

        TenantDto? organDto = null;
        if (!string.IsNullOrWhiteSpace(curOrgan))
        {
            var organ = _tenantService.FindById(long.Parse(curOrgan));
            if (organ != null)
                organDto = _tenantService.MapToDto(organ);
        }

        UserAccountDto account;
        if (accountOptional == null)
        {
            account = RegisterNewAccount(username, oauthProvider);
            var result = new SemUserDetail(null, account, organDto, _roleRepository);
            _semLogger.Info("Exit method");
            return result;
        }
        else // added by z.khosravi
        {
            if (accountOptional.User == null)
            {
                var configs = _applicationConfigService.FindActives();
                if (configs is { Count: > 0 } && SelfHosted.Equals(configs[0].ServiceType, StringComparison.OrdinalIgnoreCase))
                {
                    account = _accountService.CreateAccountUser(username, accountOptional, oauthProvider);
                    var resultNoUser = new SemUserDetail(null, account, organDto, _roleRepository);
                    _semLogger.Info("Exit method");
                    return resultNoUser;
                }
                account = null!; // در جاوا account null می‌ماند و SemUserDetail(null, null, ...) ساخته نمی‌شد — خط پایین همان مسیر است
            }
            else
            {
                account = _accountService.MapToDtoByPasswordAndUserAndRoles(accountOptional);
                if (account.UserDto != null)
                {
                    account.UserDto.LoginDisabled = accountOptional.User.LoginDisabled;
                    account.UserDto.TenantId = accountOptional.User.TenantId;
                    var resultWithUser = new SemUserDetail(account.UserDto, account, organDto, _roleRepository);
                    _semLogger.Info("Exit method");
                    return resultWithUser;
                }
            }
            var result = new SemUserDetail(null, account, organDto, _roleRepository);
            _semLogger.Info("Exit method");
            return result;
        }
    }

    private UserAccountDto? RegisterNewAccount(string username, OauthProvider? oauthProvider)
    {
        _semLogger.Info("Enter the method");
        if (oauthProvider != null)
        {
            var account = new UserAccount
            {
                ProviderId = oauthProvider.Id,
                AccountName = (username + "_" + oauthProvider.RegistrationId).ToUpper()
            };
            var user = _userService.FindByUserName(username);
            if (user != null)
                account.User = user;
            else
            {
                // change by z.khosravi
                var configs = _applicationConfigService.FindActives();
                if (configs is { Count: > 0 } && SelfHosted.Equals(configs[0].ServiceType, StringComparison.OrdinalIgnoreCase))
                {
                    return _accountService.CreateAccountUser(username, account, oauthProvider);
                }
            }
            var saved = _accountService.GetRepository().Save(account);
            _semLogger.Info("Exit method");
            return _accountService.MapToDtoByPasswordAndUserAndRoles(saved);
        }
        _semLogger.Info("Exit method");
        return null;
    }
}
