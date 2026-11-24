using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Elastic.Clients.Elasticsearch;
using SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;
using SemBackDotNet10.SecurityDomain.Infrastructure.Health;
using SemBackDotNet10.SecurityDomain.Infrastructure.OAuth;
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using SemBackDotNet10.SecurityDomain.Infrastructure.Sso;
using SemBackDotNet10.SecurityDomain.Infrastructure.Tokens;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Persistence;
using SemBackDotNet10.SecurityDomain.Providers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.SecurityDomain.Model.SemUserDetail;
using SemBackDotNet10.SecurityDomain.Configuration;
using SemBackDotNet10.SecurityDomain.Handler;
using SemBackDotNet10.Data;
using global::Common.logging;

var builder = WebApplication.CreateBuilder(args);

// Machine-local development credentials are excluded from source control and publishing.
// Environment variables and command-line options keep their usual precedence.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.Local.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables().AddCommandLine(args);
}

// Temporary development mode: allow the host to start while incomplete service
// graphs are migrated. Missing dependencies still fail when their endpoint is used.
var deferDevelopmentServiceValidation = builder.Environment.IsDevelopment() &&
    builder.Configuration.GetValue("DependencyInjection:DeferValidationInDevelopment", false);
if (deferDevelopmentServiceValidation)
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = false;
        options.ValidateScopes = true;
    });
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<SemLogger>();
var elasticsearchUrl = builder.Configuration["ExternalServices:Elasticsearch:Urls"];
if (!string.IsNullOrWhiteSpace(elasticsearchUrl))
{
    if (!Uri.TryCreate(elasticsearchUrl, UriKind.Absolute, out var elasticsearchUri))
    {
        throw new InvalidOperationException(
            "ExternalServices:Elasticsearch:Urls must be an absolute Elasticsearch URL.");
    }

    builder.Services.AddSingleton(_ =>
    {
        var settings = new ElasticsearchClientSettings(elasticsearchUri);
        var username = builder.Configuration["ExternalServices:Elasticsearch:Username"];
        var password = builder.Configuration["ExternalServices:Elasticsearch:Password"];
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password))
            settings.Authentication(new Elastic.Transport.BasicAuthentication(username, password));
        return new ElasticsearchClient(settings);
    });
    builder.Services.AddScoped<global::Common.logging.service.ActionLogService>();
    builder.Services.AddScoped<global::Common.logging.service.MeasureLogService>();
    builder.Services.AddHttpClient<global::Common.logging.IElasticsearchSearchTransport, global::Common.logging.HttpElasticsearchSearchTransport>((sp, client) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var username = configuration["ExternalServices:Elasticsearch:Username"];
        var password = configuration["ExternalServices:Elasticsearch:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Elasticsearch username and password are required for log searches.");
        new global::Common.logging.ElasticsearchConfig(elasticsearchUri, username, password,
            configuration.GetValue("ExternalServices:Elasticsearch:AllowInsecureHttp", false)).ConfigureClient(client);
    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
}
builder.Services.AddScoped<SemSecurityUtilityService>();

builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IDelegationRepository,
    SemBackDotNet10.basicDomain.Repositories.DelegationRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IMeasureUnitRepository,
    SemBackDotNet10.basicDomain.Repositories.MeasureUnitRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IOrganizationRepository,
    SemBackDotNet10.basicDomain.Repositories.OrganizationRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPerspectiveRepository,
    SemBackDotNet10.basicDomain.Repositories.PerspectiveRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlaningLevelRepository,
    SemBackDotNet10.basicDomain.Repositories.PlaningLevelRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlaningLevelUserRepository,
    SemBackDotNet10.basicDomain.Repositories.PlaningLevelUserRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlaningLevelTeamRepository,
    SemBackDotNet10.basicDomain.Repositories.PlaningLevelTeamRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlaningLevelUnitRepository,
    SemBackDotNet10.basicDomain.Repositories.PlaningLevelUnitRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlaningLevelOrganizationRepository,
    SemBackDotNet10.basicDomain.Repositories.PlaningLevelOrganizationRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository,
    SemBackDotNet10.basicDomain.Repositories.PlanPeriodRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IMainStrategyRepository,
    SemBackDotNet10.basicDomain.Repositories.MainStrategyRepository>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.Repositories.IUserRepositoryForStrategy,
    SemBackDotNet10.basicDomain.Repositories.UserRepositoryForStrategy>();
builder.Services.AddScoped<SemBackDotNet10.baseDomain.repository.IStrategyObjectRepository,
    SemBackDotNet10.baseDomain.repository.StrategyObjectRepository>();
builder.Services.AddScoped<SemBackDotNet10.baseDomain.repository.IUserRepository>(sp =>
    new SemBackDotNet10.baseDomain.repository.UserRepository(sp.GetRequiredService<SemDbContext>()));
builder.Services.AddScoped<SemBackDotNet10.baseDomain.repository.IPlanPeriodRepository>(sp =>
    new SemBackDotNet10.baseDomain.repository.PlanPeriodRepository(sp.GetRequiredService<SemDbContext>()));

builder.Services.AddScoped<SemBackDotNet10.service.impl.DelegationServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureUnitServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.OrganizationServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.PerspectiveServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.actionDomain.service.interf.IActionPlanService,
    SemBackDotNet10.actionDomain.service.impl.ActionPlanServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.actionDomain.repository.ActionPlanRepository,
    SemBackDotNet10.actionDomain.repository.ActionPlanEfRepository>();
builder.Services.AddScoped<SemBackDotNet10.actionDomain.repository.ActionPlanCatalogRepository,
    SemBackDotNet10.actionDomain.repository.ActionPlanCatalogEfRepository>();
builder.Services.AddScoped<SemBackDotNet10.actionDomain.mapper.IActionPlanCatalogMapper,
    SemBackDotNet10.actionDomain.mapper.ActionPlanCatalogMapper>();
builder.Services.AddScoped<SemBackDotNet10.measureDomain.service.interf.IMeasureService,
    SemBackDotNet10.measureDomain.service.impl.MeasureServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.baseDomain.mapper.IPerspectiveMapper,
    SemBackDotNet10.baseDomain.mapper.PerspectiveMapper>();
builder.Services.AddScoped<SemBackDotNet10.baseDomain.mapper.IStrategyObjectMapper,
    SemBackDotNet10.baseDomain.mapper.StrategyObjectMapper>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.measureDomain.service.interf.IMeasureCatalogService,
    SemBackDotNet10.measureDomain.service.impl.MeasureCatalogServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.evaluationDomain.service.interf.IEvaluationTeamHeaderLevelService,
    SemBackDotNet10.evaluationDomain.service.impl.EvaluationTeamHeaderLevelServiceImp>();
builder.Services.AddScoped<SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository,
    SemBackDotNet10.evaluationDomain.repository.EvaluationTeamHeaderLevelRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService,
    SemBackDotNet10.SecurityDomain.Service.Impl.UserRoleServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Interf.IRoleService,
    SemBackDotNet10.SecurityDomain.Service.Impl.RoleServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Interf.IPageAccessService,
    SemBackDotNet10.SecurityDomain.Service.Impl.PageAccessServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Interf.IPageService,
    SemBackDotNet10.SecurityDomain.Service.Impl.PageServiceImpl>();
builder.Services.AddScoped<global::Common.utils.userRoles.UserRolesHelpers>();
builder.Services.AddScoped<RequestSecurityContextHost>();
builder.Services.AddScoped<global::Common.utils.security.ISecurityContextHost>(sp => sp.GetRequiredService<RequestSecurityContextHost>());
builder.Services.AddScoped<global::Common.utils.userRoles.IUserRoleIdentityHost>(sp => sp.GetRequiredService<RequestSecurityContextHost>());
builder.Services.AddScoped<global::Common.utils.security.SecurityUtils>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlaningLevelTeamServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlaningLevelOrganizationServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MainStrategyServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.repository.IDashboardRepository, SemBackDotNet10.dashboardDomain.repository.DashboardRepository>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.repository.IDashboardDtoRepository, SemBackDotNet10.dashboardDomain.repository.DashboardDtoRepository>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.repository.IDashboardPCNRepository, SemBackDotNet10.dashboardDomain.repository.DashboardPCNRepository>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.repository.IDashboardReportAllLevelScoreRepository, SemBackDotNet10.dashboardDomain.repository.DashboardReportAllLevelScoreRepository>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.repository.IStrategyMapEvalByMeasureHoscoRepository, SemBackDotNet10.dashboardDomain.repository.StrategyMapEvalByMeasureHoscoRepository>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.service.interf.IDashboardService, SemBackDotNet10.dashboardDomain.service.impl.DashboardServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.dashboardDomain.domain.DashboardDomainService>();
builder.Services.AddScoped<global::Common.repository.TaskExecutorRepository>();
builder.Services.AddScoped<global::Common.repository.IPersistenceSession<global::Common.model.entity.ReportExecutedData>, SemBackDotNet10.Data.ReportExecutionPersistenceSession>();
builder.Services.AddScoped<global::Common.repository.IReportExecutionQueries, SemBackDotNet10.Data.ReportExecutionQueries>();
builder.Services.AddScoped<global::Common.repository.IPersistenceSession<global::Common.model.entity.History>, SemBackDotNet10.Data.HistoryPersistenceSession>();
builder.Services.AddScoped<global::Common.controller.AttachmentOwnerAccess>();
builder.Services.AddScoped<global::Common.controller.IAttachmentUploadService, global::Common.controller.AttachmentUploadService>();
builder.Services.AddScoped<global::Common.repository.IPersistenceSession<global::Common.model.entity.Comment>, SemBackDotNet10.Data.CommentPersistenceSession>();
builder.Services.AddScoped<global::Common.repository.HistoryRepository>();
builder.Services.AddScoped<global::Common.repository.CommentRepository>();
builder.Services.AddScoped<global::Common.service.interf.IHistoryAuditContext, SemBackDotNet10.Data.HistoryAuditContext>();
builder.Services.AddScoped<global::Common.service.interf.HistoryService, global::Common.service.impl.HistoryServiceImpl>();
builder.Services.AddScoped<global::Common.service.interf.ITaskEventStreamFactory, global::Common.service.impl.HttpTaskEventStreamFactory>();
builder.Services.AddScoped<global::Common.service.interf.TaskExecutorService, global::Common.service.impl.TaskExecutorServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.migration.SupremeBackfillService>();
if (builder.Configuration.GetValue("Database:ApplyEfMigrationsOnStartup", true))
    builder.Services.AddHostedService<SemBackDotNet10.migration.FlywayRunner>();
SemBackDotNet10.analysisDomain.AnalysisDomainServiceCollectionExtensions.AddAnalysisDomain(builder.Services);

builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IBlackListTokenRepository, SemBackDotNet10.SecurityDomain.Repository.BlackListTokenRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IControlPanelRepository, SemBackDotNet10.SecurityDomain.Repository.ControlPanelRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IOauthProviderRepository, SemBackDotNet10.SecurityDomain.Repository.OauthProviderRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IPageAccessRepository, SemBackDotNet10.SecurityDomain.Repository.PageAccessRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IPageRepository, SemBackDotNet10.SecurityDomain.Repository.PageRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IRefreshTokenRepository, SemBackDotNet10.SecurityDomain.Repository.RefreshTokenRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IRoleRepository, SemBackDotNet10.SecurityDomain.Repository.RoleRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IUserAccountRepository, SemBackDotNet10.SecurityDomain.Repository.UserAccountRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.UserRoleRepository>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Repository.IUserRoleRepository>(services =>
    services.GetRequiredService<SemBackDotNet10.SecurityDomain.Repository.UserRoleRepository>());
builder.Services.AddScoped<SemBackDotNet10.riskDomain.repository.IRiskRepository, SemBackDotNet10.riskDomain.repository.RiskRepository>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.repository.IRiskMeasureRepository, SemBackDotNet10.riskDomain.repository.RiskMeasureRepository>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.repository.IRiskActionRepository, SemBackDotNet10.riskDomain.repository.RiskActionRepository>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.service.interf.IRiskService, SemBackDotNet10.riskDomain.service.impl.RiskServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.service.interf.IRiskMeasureService, SemBackDotNet10.riskDomain.service.impl.RiskMeasureServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.service.interf.IRiskActionService, SemBackDotNet10.riskDomain.service.impl.RiskActionServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.mapper.IRiskMeasureMapper, SemBackDotNet10.riskDomain.mapper.RiskMeasureMapper>();
builder.Services.AddScoped<SemBackDotNet10.riskDomain.domain.IRiskDomainService, SemBackDotNet10.riskDomain.domain.RiskDomainService>();
builder.Services.AddScoped<SemBackDotNet10.webserviceDomain.repository.IApiKeyRepository, SemBackDotNet10.webserviceDomain.repository.ApiKeyRepository>();
builder.Services.AddScoped<SemBackDotNet10.webserviceDomain.repository.IWebServiceRepository, SemBackDotNet10.webserviceDomain.repository.WebServiceRepository>();
builder.Services.AddScoped<SemBackDotNet10.webserviceDomain.service.interf.IApiKeyService, SemBackDotNet10.webserviceDomain.service.impl.ApiKeyServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.webserviceDomain.service.interf.IWebServicesService, SemBackDotNet10.webserviceDomain.service.impl.WebServicesServiceImpl>();
var maxRequestBodySize = builder.Configuration.GetValue<long>(
    "Application:MaxRequestBodySizeBytes", 30 * 1024 * 1024);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestBodySize;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUsername = builder.Configuration["Database:Username"];
var databasePassword = builder.Configuration["Database:Password"];
if (!string.IsNullOrWhiteSpace(connectionString))
{
    var sqlConnection = new SqlConnectionStringBuilder(connectionString)
    {
        ConnectTimeout = builder.Configuration.GetValue("Database:ConnectTimeoutSeconds", 60),
        MinPoolSize = builder.Configuration.GetValue("Database:MinPoolSize", 1),
        MaxPoolSize = builder.Configuration.GetValue("Database:MaxPoolSize", 20),
        LoadBalanceTimeout = builder.Configuration.GetValue("Database:LoadBalanceTimeoutSeconds", 180)
    };
    if (!string.IsNullOrWhiteSpace(databaseUsername) && !string.IsNullOrWhiteSpace(databasePassword))
    {
        sqlConnection.UserID = databaseUsername;
        sqlConnection.Password = databasePassword;
        sqlConnection.IntegratedSecurity = false;
    }
    connectionString = sqlConnection.ConnectionString;
}

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(builder.Configuration.GetValue("Database:CommandTimeoutSeconds", 30));
            sql.MaxBatchSize(builder.Configuration.GetValue("Database:MaxBatchSize", 50));
            if (builder.Configuration.GetValue("Database:EnableRetryOnFailure", true))
            {
                sql.EnableRetryOnFailure(
                    builder.Configuration.GetValue("Database:MaxRetryCount", 3),
                    TimeSpan.FromSeconds(builder.Configuration.GetValue("Database:MaxRetryDelaySeconds", 5)),
                    null);
            }
        }));

    builder.Services.AddDbContext<SemDbContext>(options =>
        options.UseSqlServer(connectionString, sql =>
        {
            sql.CommandTimeout(builder.Configuration.GetValue("Database:CommandTimeoutSeconds", 30));
            sql.MaxBatchSize(builder.Configuration.GetValue("Database:MaxBatchSize", 50));
            if (builder.Configuration.GetValue("Database:EnableRetryOnFailure", true))
            {
                sql.EnableRetryOnFailure(
                    builder.Configuration.GetValue("Database:MaxRetryCount", 3),
                    TimeSpan.FromSeconds(builder.Configuration.GetValue("Database:MaxRetryDelaySeconds", 5)),
                    null);
            }
        }));
}

builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.ICurrentUser, SemBackDotNet10.service.impl.RequestCurrentUser>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<SemBackDotNet10.service.impl.ServiceRuntime>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.IServiceQueries, SemBackDotNet10.service.impl.ServiceQueries>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.IExternalDomainServices, SemBackDotNet10.service.impl.ExternalDomainServices>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.service.impl.ITenantMembershipQueries, SemBackDotNet10.basicDomain.service.impl.TenantMembershipQueries>();
builder.Services.AddScoped<SemBackDotNet10.basicDomain.service.ITenantService, SemBackDotNet10.basicDomain.service.impl.TenantServiceImpl>();
builder.Services.AddScoped<AuthProviderRepository>();
builder.Services.AddScoped<IAuthProviderRepository>(services =>
    services.GetRequiredService<AuthProviderRepository>());
builder.Services.AddScoped<IOAuthProviderRepository>(services =>
    services.GetRequiredService<AuthProviderRepository>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<SemUserDetail>();
builder.Services.Configure<OAuth2Options>(builder.Configuration.GetSection("OAuth2"));
builder.Services.Configure<SwitchUserOptions>(builder.Configuration.GetSection("SwitchUser"));
builder.Services.AddScoped<OAuth2AuthenticationFailureHandler>();
builder.Services.AddScoped<OAuth2AuthenticationSuccessHandler>();
builder.Services.AddScoped<SwitchUserAuthenticationSuccessHandler>();
builder.Services.AddScoped<UnAuthorizedHandler>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Infrastructure.Health.ILdapAuthenticator, TcpLdapAuthenticator>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Interfaces.ILdapAuthenticator>(services =>
    services.GetRequiredService<SemBackDotNet10.SecurityDomain.Infrastructure.Health.ILdapAuthenticator>());
builder.Services.AddScoped<IPasswordVerifier, BCryptPasswordVerifier>();
builder.Services.AddScoped<JdbcAuthenticationService>();
builder.Services.AddScoped<IAuthenticationProvider>(services => new LdapAuthenticationProvider(
    "LDAP", services.GetRequiredService<SemBackDotNet10.SecurityDomain.Interfaces.IUserAccountRepository>(),
    services.GetRequiredService<SemBackDotNet10.SecurityDomain.Interfaces.IRoleRepository>(),
    services.GetRequiredService<ILogger<LdapAuthenticationProvider>>()));
builder.Services.AddScoped<IAuthenticationProvider>(services => new LdapAuthenticationProvider(
    "LDAPOPENLDAP", services.GetRequiredService<SemBackDotNet10.SecurityDomain.Interfaces.IUserAccountRepository>(),
    services.GetRequiredService<SemBackDotNet10.SecurityDomain.Interfaces.IRoleRepository>(),
    services.GetRequiredService<ILogger<LdapAuthenticationProvider>>()));
builder.Services.AddScoped<IAuthenticationService, ProviderAuthenticationService>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Interfaces.IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IRevokedTokenService, RevokedTokenService>();
builder.Services.AddScoped<IAuthorizationService, SemAuthorizationService>();
builder.Services.AddSingleton<ISecurityAuditSink, LoggingSecurityAuditSink>();
builder.Services.AddSingleton<IBruteForceProtectionService, InMemoryBruteForceProtectionService>();
builder.Services.AddScoped<OidcProviderResolver>();

builder.Services.AddHttpClient<SemBackDotNet10.actionDomain.service.interf.ICallActionRestApiService,
    SemBackDotNet10.actionDomain.service.impl.CallActionRestApiServiceImpl>();

var ssoBaseUrl = builder.Configuration["Sso:BaseUrl"];
if (!string.IsNullOrWhiteSpace(ssoBaseUrl))
{
    builder.Services.AddHttpClient<ISsoUserClient, SsoUserClient>(client =>
    {
        client.BaseAddress = new Uri(ssoBaseUrl, UriKind.Absolute);
        client.Timeout = TimeSpan.FromSeconds(10);
    });
}

var authenticationAuthority = builder.Configuration["Authentication:Authority"];
var authenticationAudience = builder.Configuration["Authentication:Audience"];
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Infrastructure.Authentication.LocalTokenService>();
if (!string.IsNullOrWhiteSpace(authenticationAuthority) &&
    !string.IsNullOrWhiteSpace(authenticationAudience))
{
    builder.Services.AddKeycloakAuthentication(builder.Configuration);
}
else if (!string.IsNullOrWhiteSpace(authenticationAuthority) || !string.IsNullOrWhiteSpace(authenticationAudience))
{
    throw new InvalidOperationException("Both Authentication:Authority and Authentication:Audience are required when configuring JWT authentication.");
}
else if (builder.Environment.IsDevelopment())
{
    // Dev can run without an external identity provider; do not allow this fallback elsewhere.
    builder.Services.AddAuthentication();
}
else
{
    throw new InvalidOperationException(
        "Authentication:Authority and Authentication:Audience are required outside Development.");
}

builder.Services.AddScoped<global::Common.config.security.ISemIdentityResolver, SemBackDotNet10.Controllers.SemClaimsIdentityResolver>();
builder.Services.AddScoped<global::Common.config.security.ISemPageAccessStore, SemBackDotNet10.Controllers.SemRepositoryPageAccessStore>();
global::Common.config.security.SemAuthorizationRegistration.AddSemRequestUriAuthorization(builder.Services);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SemAdministrators", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Administrators"));
    options.AddPolicy("SecurityImpersonate", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("permission", "security.impersonate"));
});
builder.Services.AddCors(options => options.AddPolicy("SEM", policy =>
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
    if (origins.Length > 0)
    {
        var methods = builder.Configuration.GetSection("Cors:Methods").Get<string[]>() ?? [];
        var headers = builder.Configuration.GetSection("Cors:Headers").Get<string[]>() ?? [];
        policy.WithOrigins(origins)
            .WithMethods(methods.Length == 0 ? ["GET", "POST", "PUT", "OPTIONS", "DELETE"] : methods)
            .WithHeaders(headers.Length == 0 ? ["Authorization", "Content-Type"] : headers)
            .SetPreflightMaxAge(TimeSpan.FromSeconds(
                builder.Configuration.GetValue("Cors:PreflightMaxAgeSeconds", 3600)))
            .AllowCredentials();
    }
}));
var healthChecks = builder.Services.AddHealthChecks();
if (!string.IsNullOrWhiteSpace(connectionString))
    healthChecks.AddCheck<LdapHealthCheck>("ldap");

builder.Services.AddScoped<SemBackDotNet10.basicDomain.service.DelegationEndpointService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.SettingPersistence>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.ISettingRepository>(sp => sp.GetRequiredService<SemBackDotNet10.service.impl.SettingPersistence>());
builder.Services.AddScoped<SemBackDotNet10.service.impl.ITenantService>(sp => sp.GetRequiredService<SemBackDotNet10.service.impl.SettingPersistence>());
builder.Services.AddScoped(sp => new SemBackDotNet10.service.impl.SettingServiceImpl(
    sp.GetRequiredService<SemBackDotNet10.service.impl.ISettingRepository>(),
    sp.GetRequiredService<SemBackDotNet10.service.impl.ITenantService>(),
    sp.GetRequiredService<IConfiguration>()["Security:DefaultTenantName"] ?? ""));
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlanPeriodRepositoryAdapter>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.IPlanPeriodRepository>(sp => sp.GetRequiredService<SemBackDotNet10.service.impl.PlanPeriodRepositoryAdapter>());
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlanPeriodServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.StrategyPillarsMeasureServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.StrategyPillarServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.PositionEndpointService>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Impl.ControlPanelPageReader>();
builder.Services.AddHttpClient<SemBackDotNet10.measureDomain.service.impl.HamsooMeasureService>();
builder.Services.AddScoped<SemBackDotNet10.SecurityDomain.Service.Impl.ApplicationConfigService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.UserServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.UserDetailsReader>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.UserPageReader>();
builder.Services.AddScoped<global::Common.controller.ICartableControllerService, global::Common.service.impl.CartableControllerService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.StrategyObjectImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.StrategyObjectReadService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureObjectGridService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.ActionCatalogPageService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.ActionObjectPageService>();
builder.Services.AddScoped<SemBackDotNet10.actionDomain.repository.ActionPlanByObjectReader>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureByObjectPageService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureCountsService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.ActionPlanCountsService>();
builder.Services.AddScoped<global::Common.repository.IDesRepository>(sp =>
    new global::Common.repository.DesRepository(sp.GetRequiredService<SemDbContext>().Set<global::Common.model.entity.Des>()));
builder.Services.AddScoped<global::Common.service.interf.IDesService, global::Common.service.impl.DesServiceImpl>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureDetailsService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureRepositoryEditService>();
builder.Services.AddScoped<SemBackDotNet10.service.impl.MeasureRepositoryGridService>();
builder.Services.AddScoped<Common.service.impl.CartableConfirmationLinkService>();
builder.Services.AddScoped<Common.repository.IPersistenceSession<Common.model.entity.CartableStatus>, Common.service.impl.CartableStatusPersistence>();
builder.Services.AddScoped<Common.repository.ICartableStatusQueries, Common.service.impl.CartableStatusQueries>();
builder.Services.AddScoped<Common.repository.CartableStatusRepository>();
builder.Services.AddScoped<Common.service.interf.ICartableStatusRoleHost, Common.service.impl.CartableStatusRoleHost>();
builder.Services.AddScoped<Common.service.interf.ICartableStatusWriteScope, Common.service.impl.CartableStatusWriteScope>();
builder.Services.AddScoped<Common.service.impl.CartableStatusServiceImpl>(sp => new(
    sp.GetRequiredService<Common.repository.CartableStatusRepository>(), sp.GetRequiredService<Common.utils.userRoles.UserRolesHelpers>(),
    sp.GetRequiredService<IConfiguration>()["Security:DefaultTenantName"] ?? throw new InvalidOperationException("Security:DefaultTenantName is required."),
    sp.GetRequiredService<Common.service.interf.ICartableStatusRoleHost>(), sp.GetRequiredService<Common.service.interf.ICartableStatusWriteScope>()));
builder.Services.AddScoped<Common.service.interf.CartableStatusService>(sp => sp.GetRequiredService<Common.service.impl.CartableStatusServiceImpl>());
builder.Services.AddScoped<Common.controller.ICartableStatusControllerService>(sp => sp.GetRequiredService<Common.service.impl.CartableStatusServiceImpl>());
builder.Services.AddScoped<SemBackDotNet10.service.impl.PlaningLevelServiceImpl>();
builder.Services.AddHttpClient<global::Common.webServices.CustomRestTemplateService>();
builder.Services.AddHttpClient<global::Common.webServices.CommitteeTahavolService>();
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    string Required(string key) => config[key] is { Length: > 0 } value
        ? value : throw new InvalidOperationException($"Missing committee configuration: {key}.");
    return new global::Common.webServices.CommitteeTahavolOptions(Required("ExternalServices:CorrectiveAction:GetTokenUrl"),
        Required("ExternalServices:CorrectiveAction:Username"), Required("ExternalServices:CorrectiveAction:Password"),
        Required("ExternalServices:CorrectiveAction:CommitteeDetailsUrl"));
});
var app = builder.Build();
if (deferDevelopmentServiceValidation)
    app.Logger.LogWarning("Development startup is deferring service validation. Endpoints with incomplete dependency graphs will fail until their implementations are registered.");
SemBackDotNet10.SecurityDomain.Service.Impl.SecurityUtils.Configure(
    app.Services.GetRequiredService<IHttpContextAccessor>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var pathBase = builder.Configuration["Application:PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);
if (builder.Configuration.GetValue("Application:UseHttpsRedirection", true))
    app.UseHttpsRedirection();
app.UseCors("SEM");
app.UseMiddleware<global::Common.config.filter.WebServiceAuthenticationFilter>();
if (!string.IsNullOrWhiteSpace(connectionString))
    app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
if (!string.IsNullOrWhiteSpace(connectionString))
{
    app.UseMiddleware<LocalUserClaimsMiddleware>();
    app.UseMiddleware<TenantJwtValidationMiddleware>();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
