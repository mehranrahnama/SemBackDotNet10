using global::SemBackDotNet10.SecurityDomain.Interfaces;
using ISsoSynchronizationService = global::SemBackDotNet10.SecurityDomain.Interfaces.ISsoSynchronizationService;
using PlaningLevelUserServiceImpl = global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl;
using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using IRefreshTokenService = global::SemBackDotNet10.SecurityDomain.Service.Interf.IRefreshTokenService;
using JwtTokenProvider = global::SemBackDotNet10.SecurityDomain.tokenprovider.JwtTokenProvider;
using Token = global::SemBackDotNet10.SecurityDomain.tokenprovider.Token;
using SemLogger = global::Common.logging.SemLogger;
using CookieUtils = global::Common.utils.security.CookieUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AppDbContext = global::SemBackDotNet10.SecurityDomain.Persistence.AppDbContext;
using IAuthorizationService = global::Microsoft.AspNetCore.Authorization.IAuthorizationService;
using global::Common.config.security;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Common.exception.handler;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.Text.Json.Nodes;
using SemBackDotNet10.SecurityDomain.Model.dto.jwt;
using ProviderAuthentication = global::SemBackDotNet10.SecurityDomain.Interfaces.IAuthenticationService;
using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل AuthenticationServiceImpl.java.
/// نگاشت‌های Spring Security → دات‌نت:
///   AuthenticationManager → سرویس احراز هویت ارائه‌دهندهٔ جاری
///   GrantedAuthority       → string (نام نقش)
///   SwitchUserGrantedAuthority → رشته‌ی نشانگر JwtTokenProvider.PreviousAdministratorRole
///   HttpServletRequest/Response → HttpContext (از IHttpContextAccessor یا کنترلر)
///   RequestContextHolder   → IHttpContextAccessor (ثبت: builder.Services.AddHttpContextAccessor())
///   org.json / minidev JSONObject → System.Text.Json.Nodes.JsonObject
/// وابستگی‌های سرویس:
///   IRefreshTokenService، IBlackListTokenService، IUserRoleService، JwtTokenProvider،
///   IUserAccountService، ITenantService، IUserService، PlaningLevelUserServiceImpl،
///   ISSOSynchronizationService، IBruteForceProtectionService، ISemSecurityUtilityService،
///   SemLogger، CookieUtils، SecurityUtils، SemSwitchUserFilter، SemUserDetail، Token،
///   JwtRequest/JwtResponse/JwtRefreshRequest/JwtRefreshResponse، entityهای User/Tenant.
/// </summary>
public class AuthenticationServiceImpl
{
    private readonly int _maxDisableLoginTime;
    private readonly string _defaultTargetURL; // @Value("${sem.oauth2.authorizedRedirectUris}") — در جاوا هم استفاده نشده
    private readonly int _cookieExpiration;

    private readonly ProviderAuthentication _providerAuthentication;
    private readonly TenantContext _tenantContext;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISsoSynchronizationService _synchronizationService;
    private readonly IBlackListTokenService _blackListTokenService;
    private readonly IUserRoleService _userRoleService;
    private readonly JwtTokenProvider _jwtTokenProvider;
    private readonly IUserAccountService _accountService;
    private readonly ITenantService _tenantService;
    private readonly IUserService _userService;
    private readonly IUserAccountService _userAccountService;
    private readonly PlaningLevelUserServiceImpl _planingLevelUserService;
    private readonly IBruteForceProtectionService _bruteForceProtectionService;
    private readonly IAuthorizationService _authorizationService;
    private readonly SemLogger _semLogger;
    private readonly AppDbContext _db;

    public AuthenticationServiceImpl(
        IConfiguration configuration,
        ProviderAuthentication providerAuthentication,
        TenantContext tenantContext,
        IRefreshTokenService refreshTokenService,
        ISsoSynchronizationService synchronizationService,
        IUserRoleService userRoleService,
        JwtTokenProvider jwtTokenProvider,
        IUserAccountService accountService,
        ITenantService tenantService,
        IUserService userService,
        IUserAccountService userAccountService,
        IBlackListTokenService blackListTokenService,
        PlaningLevelUserServiceImpl planingLevelUserService,
        IBruteForceProtectionService bruteForceProtectionService,
        IAuthorizationService authorizationService,
        SemLogger semLogger,
        AppDbContext db)
    {
        // @Value ها از IConfiguration خوانده می‌شوند
        _maxDisableLoginTime = configuration.GetValue("security:disableLogin:maxMinute", 0);
        _defaultTargetURL = configuration["sem:oauth2:authorizedRedirectUris"] ?? "";
        _cookieExpiration = configuration.GetValue("sem:cookie:expiration", 0);

        _providerAuthentication = providerAuthentication;
        _tenantContext = tenantContext;
        _refreshTokenService = refreshTokenService;
        _synchronizationService = synchronizationService;
        _userRoleService = userRoleService;
        _jwtTokenProvider = jwtTokenProvider;
        _accountService = accountService;
        _tenantService = tenantService;
        _userService = userService;
        _userAccountService = userAccountService;
        _planingLevelUserService = planingLevelUserService;
        _bruteForceProtectionService = bruteForceProtectionService;
        _authorizationService = authorizationService;
        _semLogger = semLogger;
        _db = db;
    }

    public JwtResponse LoginUser(JwtRequest authenticationRequest, HttpContext context)
    {
        var userName = authenticationRequest.Username;
        _semLogger.Info("user with userName" + userName + " try to login");
        var now = DateTime.Now;
        var loginDisabledTime = _userService.LoginDisabledTime(userName);
        if (_userService.LoginDisabled(userName) && now < loginDisabledTime)
        {
            _bruteForceProtectionService.RegisterFailure(userName);
            // معادل JSONObject params با appendField
            var lockParams = new Dictionary<string, object?>
            {
                ["lockStatus"] = _userService.LoginDisabled(userName),
                ["lockTime"] = loginDisabledTime,
                ["maxDisableLoginTime"] = _maxDisableLoginTime
            };
            throw new BruteForceException("به علت تلاش ناموفق کاربر مسدود شد.", lockParams);
        }

        if (authenticationRequest.CurrentTenant is long requestedTenant &&
            (!_tenantContext.HasTenant || requestedTenant != _tenantContext.TenantId))
            throw new UnAuthorizedException();

        var principal = Authenticate(authenticationRequest.Username, authenticationRequest.Password);
        var user = _userService.FindByUserName(userName);
        user.LastSuccessfulLoginAttempt = DateTime.Now;

        var claims = new Dictionary<string, object?>
        {
            ["username"] = principal.Username,
            ["authorities"] = principal.Roles
        };
        Token token = _jwtTokenProvider.IssueToken(claims);

        Tenant? tenant;
        if (authenticationRequest.CurrentTenant != null)
            tenant = _tenantService.FindById(authenticationRequest.CurrentTenant.Value);
        else
            tenant = _tenantService.GetDefaultUserTenant(authenticationRequest.Username);
        if (tenant == null)
            throw new BusinessException("error.user.defaultOrganIdNotValid");

        if (tenant.Id != principal.TenantId)
            throw new UnAuthorizedException();

        var accountId = principal.AccountId!.Value;
        var existingTokenList = _refreshTokenService.FindAllByAccountIdAsync(accountId).GetAwaiter().GetResult();
        if (existingTokenList != null)
        {
            // deactivate all active sessions when creating new session
            _blackListTokenService.SaveAllRefreshTokensToBlackListAsync(existingTokenList).GetAwaiter().GetResult();
        }

        var refreshToken = _refreshTokenService.CreateRefreshTokenAsync(accountId, token.TokenValue).GetAwaiter().GetResult();
        var (sub, jti) = _jwtTokenProvider.ReadSubjectAndId(token.TokenValue);

        // NoSuchAlgorithmException در دات‌نت موضوعیت ندارد (SHA256 همیشه در دسترس است)
        var hashValue = _jwtTokenProvider.SetHashedCookie(jti, sub);
        CookieUtils.AddCookie(context.Response, "sessionKey", hashValue, _cookieExpiration, true, true);

        _semLogger.Info("user with userName " + userName + "login successfully");
        return new JwtResponse(_refreshTokenService.RefreshTokenDurationMilliseconds, tenant.Id, token.TokenValue, refreshToken.IssuedRefreshToken ?? throw new InvalidOperationException("Refresh token was not issued."));
    }

    public bool ValidateJwtTokenAndReportAccess(string body, HttpContext context)
    {
        _semLogger.Info("Enter the method");
        var token = context.Request.Headers["Authorization"].ToString().Split("Bearer ")[1];
        if (_jwtTokenProvider.CheckIsSsoToken(token) || _jwtTokenProvider.ValidateAndParseToken(token))
        {
            var json = JsonNode.Parse(body) as JsonObject;
            var reportUri = GetStringOrNull(json, "reportUri");
            var levelId = GetStringOrNull(json, "levelId");

            var refreshToken = _refreshTokenService.FindByTokenAsync(token).GetAwaiter().GetResult()
                ?? throw new TokenRefreshException("error.user.session.expire");
            refreshToken = _refreshTokenService.VerifyExpirationAsync(refreshToken).GetAwaiter().GetResult()
                ?? throw new TokenRefreshException("error.user.session.expire");

            var account = _accountService.FindByIdOrElseThrowAsync(refreshToken.AccountId, context.RequestAborted)
                .GetAwaiter().GetResult();
            var user = account.User ?? throw new UnAuthorizedException();
            var planningLevelAccess = _planingLevelUserService.HasAccessInLevelIdAsync(
                long.Parse(levelId!), user.UserId, context.RequestAborted).GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(reportUri) || !planningLevelAccess)
                throw new UnAuthorizedException();

            var result = _authorizationService.AuthorizeAsync(context.User,
                new SemReportAccessResource(reportUri, context.RequestAborted), "SemRequestUriAccess")
                .GetAwaiter().GetResult().Succeeded;
            _semLogger.Info("Exit method");
            return result;
        }
        _semLogger.Info("Exit method");
        return false;
    }

    public JwtRefreshResponse RefreshToken(JwtRefreshRequest request)
    {
        _semLogger.Info("Enter the method");
        var requestToken = request.RefreshToken;
        long? tenant = request.CurOrgan;

        var refreshToken = _refreshTokenService.FindByRefreshTokenAsync(requestToken).GetAwaiter().GetResult()
            ?? throw new TokenRefreshException("error.user.session.expire", requestToken);
        refreshToken = _refreshTokenService.VerifyExpirationAsync(refreshToken).GetAwaiter().GetResult()
            ?? throw new TokenRefreshException("error.user.session.expire", requestToken);
        var account = _accountService.FindByIdOrElseThrowAsync(refreshToken.AccountId).GetAwaiter().GetResult();

        var claims = new Dictionary<string, object?>
        {
            ["username"] = account.AccountName,
            ["authorities"] = GetAuthorities(account, tenant, request.OriginalUser)
        };
        Token token = _jwtTokenProvider.IssueToken(claims);
        _refreshTokenService.CreateRefreshTokenAsync(account.Id, token.TokenValue).GetAwaiter().GetResult();

        _semLogger.Info("Exit method");
        return new JwtRefreshResponse(
            new global::SemBackDotNet10.SecurityDomain.Model.SemUserDetail.Token(
                token.TokenValue, token.ExpiresInMilliseconds, token.ExpiresAt), requestToken);
    }

    /// <summary>معادل getAuthorities — GrantedAuthority → string.</summary>
    public ICollection<string> GetAuthorities(UserAccount account, long? curOrgan, string? originalUser)
    {
        _semLogger.Info("Enter the method");
        var grantedAuthorities = new List<string>();
        if (account.User != null)
        {
            grantedAuthorities.AddRange(GetAuthoritiesOfCurOrganAndApp(account, curOrgan));

            if (originalUser != null)
            {
                var orgAccount = _accountService.FindByUsernameAsync(originalUser).GetAwaiter().GetResult();
                if (orgAccount == null)
                    throw new BusinessException("error.userSwitch.session.expire");
                var userRoles = _userRoleService.FindRolesByUserId(account.User.UserId);
                if (userRoles.All(userRole => userRole.RoleName != nameof(Role.ApplicationRole.SEM_ADMIN)))
                    throw new BusinessException("error.userSwitch.session.expire");

                var tenant = _tenantService.GetDefaultUserTenant(originalUser);
                if (tenant == null)
                    throw new BusinessException("error.userSwitch.session.expire");

                var originalAuthorities = new List<string>();
                originalAuthorities.AddRange(GetAuthoritiesOfCurOrganAndApp(orgAccount, tenant.Id));

                // معادل SwitchUserGrantedAuthority(ROLE_PREVIOUS_ADMINISTRATOR, UsernamePasswordAuthenticationToken(...)):
                // نقش ویژه‌ی کاربر اصلی به‌صورت رشته‌ی نشانگر اضافه می‌شود.
                grantedAuthorities.Add(JwtTokenProvider.PreviousAdministratorRole);
            }
        }
        _semLogger.Info("Exit method");
        return grantedAuthorities;
    }

    public ICollection<string> GetAuthoritiesOfCurOrganAndApp(UserAccount account, long? curOrgan)
    {
        _semLogger.Info("Enter the method");
        var grantedAuthorities = new List<string>();
        if (account.User != null)
        {
            var userRoles = _userRoleService.FindRolesByUserId(account.User.UserId);
            if (userRoles is { Count: > 0 })
            {
                foreach (var role in userRoles)
                {
                    // معادل new SimpleGrantedAuthority(role.getRoleName())
                    grantedAuthorities.Add(role.RoleName);
                }
            }
        }
        _semLogger.Info("Exit method");
        return grantedAuthorities;
    }

    /// <summary>اعتبارسنجی نام کاربری و رمز در سازمان جاری.</summary>
    public AuthenticationResult Authenticate(string username, string password)
    {
        _semLogger.Info("a user with userName " + username + " try to login!");
        try
        {
            if (!_tenantContext.HasTenant)
                throw new System.Security.Authentication.AuthenticationException();

            var tenantId = _tenantContext.TenantId;
            var principal = _providerAuthentication.AuthenticateAsync(new AuthenticationRequest(username, password, tenantId))
                .GetAwaiter().GetResult();
            if (!principal.IsAuthenticated || principal.TenantId != tenantId ||
                principal.AccountId is not > 0 || principal.UserId is not > 0 || principal.Roles is null)
                throw new System.Security.Authentication.AuthenticationException();
            _semLogger.Info("userName " + username + " authenticated successfully!");
            return principal;
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            _semLogger.Error("userName " + username + " fail to authenticate!");
            _bruteForceProtectionService.RegisterFailure(username);
            throw new UnAuthorizedException();
        }
    }

    /// <summary>معادل isValid — خروجی JSONObject → JsonObject با valid/maxAge.</summary>
    public JsonObject IsValid(JwtRefreshRequest request)
    {
        _semLogger.Info("Enter the method");
        var requestToken = request.RefreshToken;
        var refreshToken = _refreshTokenService.FindByRefreshTokenAsync(requestToken).GetAwaiter().GetResult()
            ?? throw new TokenRefreshException("error.user.session.expire", requestToken);
        refreshToken = _refreshTokenService.VerifyExpirationAsync(refreshToken).GetAwaiter().GetResult()
            ?? throw new TokenRefreshException("error.user.session.expire", requestToken);

        long maxAge = (long)(refreshToken.ExpiresAt - DateTimeOffset.UtcNow).TotalMilliseconds;

        _semLogger.Info("Exit method");
        return new JsonObject { ["valid"] = true, ["maxAge"] = maxAge };
    }

    public void LogOff(HttpContext context)
    {
        _semLogger.Info("Enter the method");
        var identity = context.User.Identity;
        if (identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(identity.Name))
            throw new UnAuthorizedException();
        var activeAccount = _accountService.FindByUsernameAsync(identity.Name, context.RequestAborted)
            .GetAwaiter().GetResult() ?? throw new UnAuthorizedException();
        var accountId = activeAccount.Id;

        // TODO CLEAR EXPIRED TOKENS
        _blackListTokenService.RemoveExpiredTokensAsync().GetAwaiter().GetResult();

        // TODO SET TOKEN IN BLACKLIST
        var token = context.Request.Headers["Authorization"].ToString().Split("Bearer ")[1];
        var existingToken = _blackListTokenService.FindByTokenAsync(token).GetAwaiter().GetResult();
        if (existingToken == null)
        {
            _blackListTokenService.CreateBlackListTokenAsync(accountId, token).GetAwaiter().GetResult();
        }

        var usersToken = _refreshTokenService.FindAllByAccountIdAsync(accountId).GetAwaiter().GetResult();
        _blackListTokenService.SaveAllRefreshTokensToBlackListAsync(usersToken).GetAwaiter().GetResult();
        _refreshTokenService.DeleteAllByAccountIdAsync(accountId).GetAwaiter().GetResult();

        if (CookieUtils.GetCookie(context.Request, "sessionKey") != null)
        {
            CookieUtils.DeleteCookie(context.Request, context.Response, "sessionKey");
        }
        _semLogger.Info("Exit method");
    }

    public void SynchronizeWithSSO(HttpContext context, string username, Dictionary<string, string> headers)
    {
        _semLogger.Info("ENTER METHOD SYNCHRONIZE");
        var token = context.Request.Headers["Authorization"].ToString().Split("Bearer ")[1];
        _semLogger.Info("EXTRACTED TOKEN FROM REQUEST: " + token);
        if (_jwtTokenProvider.CheckIsSsoToken(token))
        {
            _semLogger.Info("SYNCHRONIZE USERS WITH SSO");
            SynchronizeUserWithSSO(context, username);
            _semLogger.Info("SETTING COOKIES FOR SSO");
            SetCookiesForSSOSignin(context, username, headers);
            _semLogger.Info("SETTING SSO TOKEN TO WHITELIST");
            SetSSOTokenToWhiteList(context, username);
            var user = _userService.FindByUserName(username);
            var lastSuccessfulLogin = DateTime.Now;
            var updated = _db.Users
                .Where(u => u.UserId == user.UserId && u.TenantId == user.TenantId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastSuccessfulLoginAttempt, lastSuccessfulLogin),
                    context.RequestAborted).GetAwaiter().GetResult();
            if (updated != 1)
                throw new UnAuthorizedException();
            user.LastSuccessfulLoginAttempt = lastSuccessfulLogin;
        }
        else
        {
            _semLogger.Info("IF FAILED -> RUNNING ELSE");
            _bruteForceProtectionService.RegisterFailure(username);
            throw new BusinessException("ارتباط با آوند برقرار نشد");
        }
    }

    public void SynchronizeUserWithSSO(HttpContext context, string username)
    {
        var token = _jwtTokenProvider.GetJwtFromRequest(context.Request);
        var checkExistingUser = _userService.FindByUserName(username);
        var checkExistinguserAccount = _userAccountService.FindByUserIdAsync(checkExistingUser.UserId, context.RequestAborted)
            .GetAwaiter().GetResult();
        if (checkExistinguserAccount == null || !checkExistinguserAccount.Any())
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnAuthorizedException();
            var userDetails = _synchronizationService.SynchronizeAsync(username, token, context.RequestAborted)
                .GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(userDetails.Username))
            {
                throw new UnAuthorizedException();
            }
        }
    }

    public void SetCookiesForSSOSignin(HttpContext context, string username, Dictionary<string, string> headers)
    {
        var token = _jwtTokenProvider.GetJwtFromRequest(context.Request);
        if (_jwtTokenProvider.CheckIsSsoToken(token))
        {
            var claims = JsonNode.Parse(_jwtTokenProvider.ParseToken("payload", token)) as JsonObject;
            if (CookieUtils.GetCookie(context.Request, "sessionKey") == null)
            {
                var userName = claims?["username"]?.GetValue<string>();
                var sid = claims?["sid"]?.GetValue<string>();
                var sessionKey = _jwtTokenProvider.SetHashedCookie(sid, userName);
                CookieUtils.AddCookie(context.Response, "sessionKey", sessionKey, _cookieExpiration, true, true);
            }
        }
        else
        {
            throw new UnAuthorizedException();
        }
    }

    public void SetSSOTokenToWhiteList(HttpContext context, string username)
    {
        var requestToken = context.Request.Headers["Authorization"].ToString().Split("Bearer ")[1];
        var account = _accountService.FindByUsernameAsync(username + "_IRISA_LDAP", context.RequestAborted)
            .GetAwaiter().GetResult() ?? throw new UnAuthorizedException();
        var accountId = account.Id;
        var existingToken = _refreshTokenService.FindByTokenAsync(requestToken).GetAwaiter().GetResult();
        if (existingToken == null)
        {
            _refreshTokenService.CreateRefreshTokenAsync(accountId, requestToken).GetAwaiter().GetResult();
        }
    }

    /// <summary>معادل jsonObject.isNull(key) ? null : getString(key).</summary>
    private static string? GetStringOrNull(JsonObject? json, string key)
    {
        return json?.TryGetPropertyValue(key, out var node) == true && node is not null
            ? node.GetValue<string>()
            : null;
    }
}
